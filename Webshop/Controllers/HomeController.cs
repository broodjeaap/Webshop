using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {

        private WebshopContext db = new WebshopContext();

        public ActionResult Index()
        {
            return View(db.Products.Take(100).AsEnumerable());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Product(int id)
        {
            return View(db.Products.Find(id));
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SideBar()
        {
            var categories = db.Products.Select(p => p.Category).Distinct().ToList();
            StringBuilder sb = new StringBuilder(categories.Count * 10);
            foreach (var c in categories)
            {
                sb.Append(c);
                sb.Append("<br />");
            }
            return Content(sb.ToString());
        }
    }
}
