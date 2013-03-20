﻿using System;
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

        public ActionResult Index(string category = "", string subcat1 = "", string subcat2 = "")
        {
            ViewBag.category = category;
            ViewBag.subcat1 = subcat1;
            ViewBag.subcat2 = subcat2;
            List<Product> filteredProducts = db.Products.ToList();
            if (!category.Equals(""))
            {
                filteredProducts = filteredProducts.Where(p => p.Category.Equals(category)).ToList();
            }
            if (!subcat1.Equals(""))
            {
                filteredProducts = filteredProducts.Where(p => p.SubCat1.Equals(subcat1)).ToList();
            }
            if (!subcat2.Equals(""))
            {
                filteredProducts = filteredProducts.Where(p => p.SubCat2.Equals(subcat2)).ToList();
            }
            return View(filteredProducts.Take(20));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Product(int id, string category = "", string subcat1 = "", string subcat2 = "")
        {
            ViewBag.category = category;
            ViewBag.subcat1 = subcat1;
            ViewBag.subcat2 = subcat2;
            return View(db.Products.Find(id));
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SideBar(string category = "", string subcat1 = "", string subcat2 = "")
        {
            var categories = db.Products.Select(p => p.Category).Distinct().ToList();
            StringBuilder sb = new StringBuilder("<table id=\"category_table\">",categories.Count * 10);
            foreach (var c in categories)
            {
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<a href=\"/Home?category=");
                sb.Append(HttpUtility.HtmlEncode(c));
                sb.Append("\">");
                sb.Append(c);
                sb.Append("</a>");
                sb.Append("</td>");
                sb.Append("</tr>");
                if (c.Equals(category))
                {
                    var subCategories1Products = db.Products.Where(p => p.Category.Equals(category));
                    var subCategories1 = subCategories1Products.Select(p => p.SubCat1).Distinct().ToList();
                    foreach (var sc1 in subCategories1)
                    {
                        sb.Append("<tr>");
                        sb.Append("<td>");
                        sb.Append("<a class=\"subcat1\" href=\"/Home?category=");
                        sb.Append(HttpUtility.HtmlEncode(c));
                        sb.Append("&subcat1=");
                        sb.Append(HttpUtility.HtmlEncode(sc1));
                        sb.Append("\">");
                        sb.Append(sc1);
                        sb.Append("</a>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        if (sc1.Equals(subcat1))
                        {
                            var subCategories2Products = subCategories1Products.Where(p => p.SubCat1.Equals(sc1));
                            var subCategories2 = subCategories2Products.Select(p => p.SubCat2).Distinct().ToList();
                            foreach (var sc2 in subCategories2)
                            {
                                if(sc2.Equals("")){ //blegh
                                    continue;
                                }
                                sb.Append("<tr>");
                                sb.Append("<td>");
                                sb.Append("<a class=\"subcat2\" href=\"/Home?category=");
                                sb.Append(HttpUtility.HtmlEncode(c));
                                sb.Append("&subcat1=");
                                sb.Append(HttpUtility.HtmlEncode(sc1));
                                sb.Append("&subcat2=");
                                sb.Append(HttpUtility.HtmlEncode(sc2));
                                sb.Append("\">");
                                sb.Append(sc2);
                                sb.Append("</a>");
                                sb.Append("</td>");
                                sb.Append("</tr>");
                            }
                        }
                    }
                }
            }
            sb.Append("</table>");
            return Content(sb.ToString());
        }
    }
}
