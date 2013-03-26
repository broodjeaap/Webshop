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

        private WebshopContext db = new WebshopContext();
        //
        // GET: /Shoppingcart/

        public ActionResult Index()
        {
            return View(db.Users.Find(WebSecurity.CurrentUserId).ShoppingCartItems);
        }

        public ActionResult Update(int id, int quantity, bool ajax = false)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                if (ajax)
                {
                    return Content("Fail, no such product");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            var productsInShoppingCart = db.Users.Find(WebSecurity.CurrentUserId).ShoppingCartItems;
            var productInShoppingCart = productsInShoppingCart.Where(p => p.ProductID == product.ProductID);
            if (productInShoppingCart.Count() == 1)
            {
                productInShoppingCart.First().Quantity = quantity;
            }
            else
            {
                var item = new ShoppingCartItem();
                item.ProductID = product.ProductID;
                item.Product = product;
                item.UserID = WebSecurity.CurrentUserId;
                item.Quantity = quantity;
                productsInShoppingCart.Add(item);
            }
            db.SaveChanges();

            if (ajax)
            {
                return Content("OK");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Add(int id, int quantity = 1, bool ajax = false)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                if (ajax)
                {
                    return Content("Fail, no such product");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            var productsInShoppingCart = db.Users.Find(WebSecurity.CurrentUserId).ShoppingCartItems;
            var productInShoppingCart = productsInShoppingCart.Where(p => p.ProductID == product.ProductID);
            if (productInShoppingCart.Count() == 1)
            {
                productInShoppingCart.First().Quantity += quantity;
            }
            else
            {
                var item = new ShoppingCartItem();
                item.ProductID = product.ProductID;
                item.Product = product;
                item.UserID = WebSecurity.CurrentUserId;
                item.Quantity = quantity;
                productsInShoppingCart.Add(item);
            }
            db.SaveChanges();
            if (ajax)
            {
                return Content("OK");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Delete(int id, int quantity = 1, bool ajax = false)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                if (ajax)
                {
                    return Content("Fail, no such product");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            var productsInShoppingCart = db.Users.Find(WebSecurity.CurrentUserId).ShoppingCartItems;
            var productInShoppingCart = productsInShoppingCart.Where(p => p.ProductID == product.ProductID);
            if (productInShoppingCart.Count() == 1)
            {
                var scp = productInShoppingCart.First();
                scp.Quantity -= quantity;
                if (scp.Quantity <= 0)
                {
                    db.ShoppingCartItems.Remove(scp);
                }
            }
            else
            {
                if (ajax)
                {
                    return Content("Fail, product not in shopping cart");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            db.SaveChanges();
            if (ajax)
            {
                return Content("OK");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult PickAddressOrder()
        {
            return View(db.Users.Find(WebSecurity.CurrentUserId).Addresses);
        }

        public ActionResult OrderItemsToAddress(int id)
        {
            var result = db.Addresses.Where(a => a.UserID == WebSecurity.CurrentUserId).Where(a => a.AddressID == id);
            if (result.Count() == 1)
            {
                var user = db.Users.Find(WebSecurity.CurrentUserId);
                var order = new Order();
                order.User = user;
                order.UserID = WebSecurity.CurrentUserId;
                order.AddressID = id;
                order.OrderTime = DateTime.Now;
                foreach (var i in user.ShoppingCartItems)
                {
                    var oi = new OrderItem();
                    oi.Order = order;
                    oi.OrderID = order.OrderID;
                    oi.Product = i.Product;
                    oi.ProductID = i.ProductID;
                    oi.Quantity = i.Quantity;
                    order.OrderItems.Add(oi);
                }
                var items = user.ShoppingCartItems.ToList();
                for (var a = items.Count() - 1; a >= 0; --a)
                {
                    db.ShoppingCartItems.Remove(items[a]);
                }
                db.Orders.Add(order);
                db.SaveChanges();
                return View();
            }
            return RedirectToAction("Index");
        }
    }
}
