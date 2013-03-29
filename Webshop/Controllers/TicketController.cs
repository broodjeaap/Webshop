using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webshop.Models;

namespace Webshop.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private WebshopContext db = new WebshopContext();

        public ActionResult Index(int id)
        {
            return View(db.Tickets.Find(id));
        }
    }
}
