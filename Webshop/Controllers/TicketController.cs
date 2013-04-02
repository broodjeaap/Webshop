﻿using System;
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
        private WebshopContext db = new WebshopContext();

        public ActionResult Index()
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            if (user.UserType == UserType.Customer)
            {
                return View(user.Tickets.OrderBy(t => t.TicketCreationDate).ToList());
            }
            if (user.UserType == UserType.Help)
            {
                return View("HelpIndex", user.Tickets.OrderBy(t => t.TicketCreationDate).ToList());
            }
            if (user.UserType == UserType.Service)
            {
                return View("ServiceIndex", db.Tickets.Where(t => t.TicketState == TicketState.New).OrderBy(t => t.TicketCreationDate).ToList());
            }
            return View("AdminIndex", db.Tickets.OrderBy(t => t.TicketCreationDate).ToList());
        }

        public ActionResult Ticket(int id)
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            var ticket = db.Tickets.Find(id);
            if (!user.Tickets.Contains(ticket) && user.UserType != UserType.Admin)
            {
                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ticket(TicketComment ticketComment)
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            ticketComment.UserID = user.UserID;
            if (ticketComment.Text != null && !ticketComment.Text.Trim().Equals("") && (db.Tickets.Find(ticketComment.TicketID).Users.Contains(user) || user.UserType == UserType.Admin))
            {
                ticketComment.User = user;
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
            }
            return View(db.Tickets.Find(ticketComment.TicketID));
        }

        public ActionResult EventHistory(int id)
        {
            return View(db.Tickets.Find(id));
        }

        public ActionResult Assign(int id)
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Index");
            }
            var Ticket = db.Tickets.Find(id);
            ViewBag.Ticket = Ticket;
            return View(db.Users.Where(u => u.UserType == UserType.Help && u.UserID != user.UserID));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(int TicketID, int UserID)
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            if (user.UserID == UserID)
            {
                return RedirectToAction("Index");
            }
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Index");
            }
            var assignedToUser = db.Users.Find(UserID);
            if (assignedToUser.UserType != UserType.Help)
            {
                return RedirectToAction("Index");
            }
            var ticket = db.Tickets.Find(TicketID);
            if (!assignedToUser.Tickets.Contains(ticket))
            {
                assignedToUser.Tickets.Add(ticket);
                ticket.Users.Add(assignedToUser);
                ticket.TicketState = TicketState.Open;

                var ticketEvent = new TicketEvent();
                ticketEvent.NewTicketState = TicketState.Open;
                ticketEvent.text = "Ticket " + ticket.TicketID + " ( " + ticket.TicketTitle + " ) assigned to " + assignedToUser.Email + " by " + user.Email;
                ticketEvent.TicketID = ticket.TicketID;
                ticketEvent.Ticket = ticket;
                db.TicketEvents.Add(ticketEvent);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
