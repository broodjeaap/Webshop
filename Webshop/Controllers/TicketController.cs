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
        private IWebshopDAO dao;
        private IWebshopDSO dso;

        public TicketController(IWebshopDAO dao, IWebshopDSO dso)
        {
            this.dao = dao;
            this.dso = dso;
        }

        public ActionResult Index()
        {
            var user = dao.getCurrentUser();
            switch (user.UserType)
            {
                case UserType.Admin:
                    {
                        return View("AdminIndex", dao.getAllTicketsOrderedByDate());
                    }
                case UserType.Customer:
                    {
                        return View(dao.getUserTicketsOrderedByDate());
                    }
                case UserType.Help:
                    {
                        return View("HelpIndex", dao.getUserTicketsOrderedByDate());
                    }
                default:
                    {
                        return View("ServiceIndex", dao.getNewTicketsOrderedByDate());
                    }
            }            
        }

        public ActionResult Ticket(int id)
        {
            if (dso.currentUserCanSeeTicket(id))
            {
                return RedirectToAction("Index");
            }
            dso.setTicketLastViewedByCurrentUserToNow(id);
            return View(dao.getTicket(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ticket(TicketComment ticketComment)
        {
            dso.postCommentOnTicket(ticketComment);
            return RedirectToAction("Ticket", new { id = ticketComment.TicketID } );
        }

        public ActionResult TicketStateChange(int id, string type)
        {
            dso.changeTicketState(id, type);
            return RedirectToAction("Ticket", new { id = id });
        }

        public ActionResult EventHistory(int id)
        {
            return View(dao.getTicket(id));
        }

        public ActionResult Assign(int id)
        {
            var user = dao.getCurrentUser();
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Index");
            }
            var Ticket = dao.getTicket(id);
            ViewBag.Ticket = Ticket;
            return View(dao.getHelpUsers());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(int TicketID, int UserID)
        {
            dso.assignUserToTicket(UserID, TicketID);
            return RedirectToAction("Index");
        }
    }
}
