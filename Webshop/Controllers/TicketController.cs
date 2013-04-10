using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using Webshop.Models;

namespace Webshop.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly WebshopDAO db;

        public TicketController(WebshopDAO db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            var user = db.getUsers().Find(WebSecurity.CurrentUserId);
            switch (user.UserType)
            {
                case UserType.Admin:
                    {
                        return View("AdminIndex", db.getTickets().OrderBy(t => t.TicketCreationDate).ToList());
                    }
                case UserType.Customer:
                    {
                        return View(user.UserTicketLinks.OrderBy(utl => utl.Ticket.TicketCreationDate).ToList());
                    }
                case UserType.Help:
                    {
                        return View("HelpIndex", user.UserTicketLinks.OrderBy(utl => utl.Ticket.TicketCreationDate).ToList());
                    }
                default:
                    {
                        return View("ServiceIndex", db.getTickets().Where(t => t.TicketState == TicketState.New).OrderBy(t => t.TicketCreationDate).ToList());
                    }
            }            
        }

        public ActionResult Ticket(int id)
        {
            var user = db.getUsers().Find(WebSecurity.CurrentUserId);
            var ticket = db.getTickets().Find(id);
            if (user.UserTicketLinks.Where(utl => utl.TicketID == ticket.TicketID && utl.UserID == user.UserID).Count() == 0 && user.UserType != UserType.Admin)
            {
                return RedirectToAction("Index");
            }
            if (user.UserType != UserType.Admin)
            {
                db.getUserTicketLinks().Find(user.UserID, ticket.TicketID).LastViewed = DateTime.Now;
                db.SaveChanges();
            }
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ticket(TicketComment ticketComment)
        {
            var user = db.getUsers().Find(WebSecurity.CurrentUserId);
            ticketComment.UserID = user.UserID;
            if (ticketComment.Text != null && !ticketComment.Text.Trim().Equals("") && (db.getUserTicketLinks().Find(user.UserID, ticketComment.TicketID) != null || user.UserType != UserType.Customer))
            {
                ticketComment.User = user;
                db.getTicketComments().Add(ticketComment);
                db.SaveChanges();
            }
            if (user.UserType != UserType.Admin)
            {
                db.getUserTicketLinks().Find(user.UserID, ticketComment.TicketID).LastViewed = DateTime.Now;
                db.getTickets().Find(ticketComment.TicketID).LastCommentDate = DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("Ticket", new { id = ticketComment.TicketID } );
        }

        public ActionResult TicketStateChange(int id, string type)
        {
            var user = db.getUsers().Find(WebSecurity.CurrentUserId);
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Ticket", new { id = id } );
            }
            if (user.UserTicketLinks.Where(utl => utl.TicketID == id).Count() != 1 || user.UserType == UserType.Admin)
            {
                return RedirectToAction("Index", new { id = id });
            }
            var ticket = db.getTickets().Find(id);
            ticket.TicketState = (TicketState)(Enum.Parse(typeof(TicketState), type));
            db.Entry(ticket).State = System.Data.EntityState.Modified;
            var te = new TicketEvent();
            te.NewTicketState = ticket.TicketState;
            te.text = "Ticket " + ticket.TicketID + " ( " + ticket.TicketTitle + " ) was changed to state " + ticket.TicketState + " by " + user.Email;
            te.TicketID = id;
            te.Ticket = ticket;
            db.getTicketEvents().Add(te);
            if (user.UserType != UserType.Admin)
            {
                db.getUserTicketLinks().Find(user.UserID, id).LastViewed = DateTime.Now;
                ticket.LastCommentDate = DateTime.Now;
            }
            db.SaveChanges();
            return RedirectToAction("Ticket", new { id = id });
        }

        public ActionResult EventHistory(int id)
        {
            return View(db.getTickets().Find(id));
        }

        public ActionResult Assign(int id)
        {
            var user = db.getUsers().Find(WebSecurity.CurrentUserId);
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Index");
            }
            var Ticket = db.getTickets().Find(id);
            ViewBag.Ticket = Ticket;
            return View(db.getUsers().Where(u => u.UserType == UserType.Help && u.UserID != user.UserID));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(int TicketID, int UserID)
        {
            var user = db.getUsers().Find(WebSecurity.CurrentUserId);
            if (user.UserID == UserID)
            {
                return RedirectToAction("Index");
            }
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Index");
            }
            var assignedToUser = db.getUsers().Find(UserID);
            if (assignedToUser.UserType != UserType.Help)
            {
                return RedirectToAction("Index");
            }
            var ticket = db.getTickets().Find(TicketID);
            if (!assignedToUser.UserTicketLinks.Select(utl => utl.Ticket).Contains(ticket))
            {
                var userTicketLink = new UserTicketLink();
                userTicketLink.UserID = assignedToUser.UserID;
                userTicketLink.User = assignedToUser;
                userTicketLink.TicketID = ticket.TicketID;
                userTicketLink.Ticket = ticket;
                db.getUserTicketLinks().Add(userTicketLink);
                ticket.TicketState = TicketState.Open;

                var ticketEvent = new TicketEvent();
                ticketEvent.NewTicketState = TicketState.Open;
                ticketEvent.text = "Ticket " + ticket.TicketID + " ( " + ticket.TicketTitle + " ) assigned to " + assignedToUser.Email + " by " + user.Email;
                ticketEvent.TicketID = ticket.TicketID;
                ticketEvent.Ticket = ticket;
                db.getTicketEvents().Add(ticketEvent);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
