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
        private IWebshopDAO dao = new WebshopDAO();
        private List<Product> empty = new List<Product>();

        public ActionResult Index(string query = "", int page = 1, int perPage = 10)
        {
            ViewBag.currentPage = page;
            ViewBag.perPage = perPage;
            ViewBag.query = query;
            page = page - 1;

            var filteredProducts = dao.getSearchResults(query);
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
