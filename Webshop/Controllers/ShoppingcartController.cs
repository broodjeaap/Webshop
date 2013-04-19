using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using Webshop.Filters;
using Webshop.Models;

namespace Webshop.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ShoppingcartController : Controller
    {
        private IWebshopDAO dao;
        private IWebshopDSO dso;

        public ShoppingcartController(IWebshopDAO dao, IWebshopDSO dso)
        {
            this.dao = dao;
            this.dso = dso;
        }

        //
        // GET: /Shoppingcart/

        public ActionResult Index()
        {
            return View(dao.getCurrentUsersShoppingCartItems());
        }

        public ActionResult Update(int id, int quantity)
        {
            dso.updateShoppingCartItem(id, quantity);
            return RedirectToAction("Index");
        }

        public ActionResult Add(int id, int quantity = 1)
        {
            dso.addShoppingCartItem(id, quantity);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id, int quantity = 1)
        {
            dso.deleteShoppingCartItem(id, quantity);
            return RedirectToAction("Index");
        }

        public ActionResult PickAddressOrder()
        {
            return View(dao.getCurrentUser().Addresses);
        }

        public ActionResult OrderItemsToAddress(int id)
        {
            dso.placeOrderToAddress(id);
            return RedirectToAction("Index");
        }
    }
}
