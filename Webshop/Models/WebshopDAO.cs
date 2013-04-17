using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace Webshop.Models
{
    public class WebshopDAO : IWebshopDAO
    {

        private WebshopContext db;

        public WebshopDAO()
        {
            db = new WebshopContext();
        }

        public WebshopDAO(WebshopContext db)
        {
            this.db = db;
        }

        public List<Product> getProducts(string category = "", string subcat1 = "", string subcat2 = "", int page = 1, int perPage = 10)
        {
            var filteredProducts = db.Products.AsQueryable();
            if (!category.Equals(""))
            {
                filteredProducts = filteredProducts.Where(p => p.Category.Equals(category));
            }
            if (!subcat1.Equals(""))
            {
                filteredProducts = filteredProducts.Where(p => p.SubCat1.Equals(subcat1));
            }
            if (!subcat2.Equals(""))
            {
                filteredProducts = filteredProducts.Where(p => p.SubCat2.Equals(subcat2));
            }
            return filteredProducts.ToList();
        }

        public List<Product> getSearchResults(string query)
        {
            var filteredProducts = db.Products.AsQueryable();
            var keywords = query.Split(' ');
            foreach (var keyword in keywords)
            {
                if (keyword.StartsWith("-"))
                {
                    filteredProducts = filteredProducts.Where(p => !p.Name.ToLower().Contains(keyword.Substring(1).ToLower()));
                }
                else
                {
                    filteredProducts = filteredProducts.Where(p => p.Name.ToLower().Contains(keyword.ToLower()));
                }
            }
            return filteredProducts.ToList();
        }

        public Product getProduct(int id)
        {
            return db.Products.Find(id);
        }

        public User getCurrentUser()
        {
            if (!WebSecurity.IsAuthenticated)
            {
                return null;
            }
            return db.Users.Find(WebSecurity.CurrentUserId);
        }

        public int getCurrentUserId()
        {
            return (WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserId : -1);
        }

        public User getUser(int id)
        {
            return db.Users.Find(id);
        }

        public bool getCurrentUserIsAdmin()
        {
            var user = getCurrentUser();
            return (user != null && (user.UserType == UserType.Admin || user.UserType == UserType.Service));
        }

        public Order getOrder(int id)
        {
            var user = getCurrentUser();
            return user.Orders.FirstOrDefault(o => o.OrderID == id);
        }

        public List<ShoppingCartItem> getCurrentUsersShoppingCartItems()
        {
            return getCurrentUser().ShoppingCartItems.ToList();
        }

        public List<Ticket> getAllTicketsOrderedByDate()
        {
            return db.Tickets.OrderBy(t => t.TicketCreationDate).ToList();
        }

        public List<UserTicketLink> getUserTicketsOrderedByDate()
        {
            return getCurrentUser().UserTicketLinks.OrderBy(utl => utl.Ticket.TicketCreationDate).ToList();
        }

        public List<Ticket> getNewTicketsOrderedByDate()
        {
            return db.Tickets.Where(t => t.TicketState == TicketState.New).OrderBy(t => t.TicketCreationDate).ToList();
        }

        public Ticket getTicket(int id)
        {
            return db.Tickets.Find(id);
        }

        public List<User> getHelpUsers()
        {
            return db.Users.Where(u => u.UserType == UserType.Help && u.UserID != WebSecurity.CurrentUserId).ToList();
        }

        public List<User> getNonCustomerUsers()
        {
            return db.Users.Where(u => u.UserType != UserType.Customer).ToList();
        }
    }
}