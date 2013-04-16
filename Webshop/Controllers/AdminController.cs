using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using Webshop.Models;

namespace Webshop.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        WebshopContext db = new WebshopContext();
        IWebshopDAO dao = new WebshopDAO();

        public ActionResult Index()
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(db.Users.Where(u => u.UserType != UserType.Customer).ToList());
        }

        [HttpPost]
        public ActionResult Index(string Email, string type)
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            var user = new User();
            user.Email = Email;
            user.UserType = (UserType)Enum.Parse(typeof(UserType), type);
            db.Users.Add(user);
            db.SaveChanges();
            WebSecurity.CreateAccount(Email, "password");
            return RedirectToAction("Index");
        }

        public ActionResult Product(int id)
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(db.Products.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Product(Product product)
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Product");
        }

        public ActionResult UserTypePost(int id, string type)
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            var user = dao.getUser(id);
            var enumType = (UserType)Enum.Parse(typeof(UserType), type);
            user.UserType = enumType;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
