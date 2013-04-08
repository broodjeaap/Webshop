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

        public ActionResult Index()
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(db.Users.Where(u => u.UserType != UserType.Customer).ToList());
        }

        [HttpPost]
        public ActionResult Index(string Email, string type)
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Index", "Home");
            }
            user = new User();
            user.Email = Email;
            user.UserType = (UserType)Enum.Parse(typeof(UserType), type);
            db.Users.Add(user);
            db.SaveChanges();
            WebSecurity.CreateAccount(Email, "password");
            return RedirectToAction("Index");
        }

        public ActionResult Product(int id)
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(db.Products.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Product(Product product)
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            if (user.UserType == UserType.Customer)
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
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            if (user.UserType == UserType.Customer)
            {
                return RedirectToAction("Index", "Home");
            }
            user = db.Users.Find(id);
            var enumType = (UserType)Enum.Parse(typeof(UserType), type);
            user.UserType = enumType;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
