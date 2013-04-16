using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class SearchController : Controller
    {
        private WebshopContext db = new WebshopContext();
        private List<Product> empty = new List<Product>();

        public ActionResult Index(string query = "", int page = 1, int perPage = 10)
        {
            ViewBag.currentPage = page;
            ViewBag.perPage = perPage;
            if (query.Equals(""))
            {
                return View(empty);
            }
            ViewBag.query = query;
            page = page - 1;

            List<Product> filteredProducts = db.Products.ToList();
            var keywords = query.Split(' ');
            foreach (var keyword in keywords)
            {
                if (keyword.StartsWith("-"))
                {
                    filteredProducts = filteredProducts.Where(p => !p.Name.ToLower().Contains(keyword.Substring(1).ToLower())).ToList();
                }
                else
                {
                    filteredProducts = filteredProducts.Where(p => p.Name.ToLower().Contains(keyword.ToLower())).ToList();
                }
            }
            var numberOfPages = filteredProducts.Count() / (float)perPage;
            var numberOfPagesI = (int)numberOfPages;
            if (numberOfPages > numberOfPagesI)
            {
                numberOfPagesI++;
            }
            ViewBag.numberOfPages = numberOfPagesI;
            return View(filteredProducts.Skip(perPage * page).Take(perPage));
        }
    }
}
