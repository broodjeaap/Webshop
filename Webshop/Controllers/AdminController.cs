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
        private IWebshopDAO dao = new WebshopDAO();
        private IWebshopDSO dso = new WebshopDSO();

        public ActionResult Index()
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(dao.getNonCustomerUsers());
        }

        [HttpPost]
        public ActionResult Index(string Email, string type)
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            dso.adminCreateUser(Email, "password", type);
            return RedirectToAction("Index");
        }

        public ActionResult Product(int id)
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(dao.getProduct(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Product(Product product)
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            dso.editProduct(product);
            return RedirectToAction("Product");
        }

        public ActionResult UserTypePost(int id, string type)
        {
            if (!dao.getCurrentUserIsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            dso.changeUserType(id, type);
            return RedirectToAction("Index");
        }
    }
}
