using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace Webshop.Models
{
    public class WebshopDSO : IWebshopDSO
    {
        private WebshopContext db = new WebshopContext();

        public bool createUser(string email, string password)
        {
            return createUser(email, password, UserType.Customer);
        }

        public bool createUser(string email, string password, string type)
        {
            return createUser(email, password, (UserType)(Enum.Parse(typeof(UserType), type)));
        }

        public bool createUser(string email, string password, UserType type)
        {
            if (WebSecurity.UserExists(email))
            {
                return false;
            }
            var user = new User();
            user.Email = email;
            user.UserType = type;
            db.Users.Add(user);
            db.SaveChanges();
            WebSecurity.CreateAccount(email, password);
            WebSecurity.Login(email, password);
            return true;
        }

        public bool adminCreateUser(string email, string password, string type)
        {
            return adminCreateUser(email, password, (UserType)(Enum.Parse(typeof(UserType), type)));
        }

        public bool adminCreateUser(string email, string password, UserType type)
        {
            if (!getCurrentUserIsAdmin())
            {
                return false;
            }
            if (WebSecurity.UserExists(email))
            {
                return false;
            }
            var user = new User();
            user.Email = email;
            user.UserType = type;
            db.Users.Add(user);
            db.SaveChanges();
            WebSecurity.CreateAccount(email, password);
            return true;
        }

        public bool changeUserType(int id, string type)
        {
            return changeUserType(getUser(id), type);
        }

        public bool changeUserType(User user, string type)
        {
            return changeUserType(user, (UserType)(Enum.Parse(typeof(UserType), type)));
        }

        public bool changeUserType(User user, UserType type)
        {
            user.UserType = type;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        public bool addAddressToCurrentUser(Address address)
        {
            address.UserID = getCurrentUserId();
            getCurrentUser().Addresses.Add(address);
            db.SaveChanges();
            return true;
        }

        public bool changeAddress(Address address)
        {
            address.UserID = getCurrentUserId();
            db.Entry(address).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        public bool editProduct(Product product)
        {
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        public bool createTicket(Ticket ticket)
        {
            ticket.OwnerUserID = getCurrentUserId();
            db.Tickets.Add(ticket);
            var user = getCurrentUser();

            var userTicketLink = new UserTicketLink();
            userTicketLink.UserID = user.UserID;
            userTicketLink.User = user;
            userTicketLink.TicketID = ticket.TicketID;
            userTicketLink.Ticket = ticket;
            db.UserTicketLinks.Add(userTicketLink);

            var ticketEvent = new TicketEvent();
            ticketEvent.text = "Ticket created";
            db.TicketEvents.Add(ticketEvent);
            db.SaveChanges();
            return true;
        }

        public List<string> getDistinctCategories()
        {
            return db.Products.Select(p => p.Category).Distinct().ToList();
        }

        public List<string> getDistinctSubCat1(string category)
        {
            return db.Products.Where(p => p.Category.Equals(category)).Select(p => p.SubCat1).Distinct().ToList();
        }
        public List<string> getDistinctSubCat2(string category, string subcat1)
        {
            return db.Products.Where(p => p.Category.Equals(category) && p.SubCat1.Equals(subcat1)).Select(p => p.SubCat2).Distinct().ToList();
        }

        public bool addShoppingCartItem(int id, int quantity = 1)
        {
            var product = getProduct(id);
            if (product == null)
            {
                return false;
            }
            var productsInShoppingCart = getCurrentUsersShoppingCartItems();
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
                item.UserID = getCurrentUserId();
                item.Quantity = quantity;
                db.ShoppingCartItems.Add(item);
            }
            db.SaveChanges();
            return true;
        }

        public bool updateShoppingCartItem(int id, int quantity)
        {
            var product = getProduct(id);
            if (product == null)
            {
                return false;
            }

            var productsInShoppingCart = getCurrentUsersShoppingCartItems();
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
                item.UserID = getCurrentUserId();
                item.Quantity = quantity;
                productsInShoppingCart.Add(item);
            }
            db.SaveChanges();

            return true;
        }

        public bool deleteShoppingCartItem(int id, int quantity)
        {
            var product = getProduct(id);
            if (product == null)
            {
                return false;
            }
            var productsInShoppingCart = getCurrentUsersShoppingCartItems();
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
                return false;
            }
            db.SaveChanges();
            return true;
        }

        public bool placeOrderToAddress(int id)
        {
            var result = getCurrentUser().Addresses.Where(a => a.AddressID == id);
            if (result.Count() == 1)
            {
                if (result.First().UserID != getCurrentUserId())
                {
                    return false;
                }
                var user = getCurrentUser();
                var order = new Order();
                order.User = user;
                order.UserID = user.UserID;
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
            }
            return true;
        }

        public bool currentUserCanSeeTicket(int id)
        {
            return currentUserCanSeeTicket(getTicket(id));
        }

        public bool currentUserCanSeeTicket(Ticket ticket)
        {
            var user = getCurrentUser();
            return (user.UserTicketLinks.Where(utl => utl.TicketID == ticket.TicketID && utl.UserID == user.UserID).Count() == 0 && user.UserType != UserType.Admin);
        }

        public bool setTicketLastViewedByCurrentUserToNow(int ticketID)
        {
            var user = getCurrentUser();
            if (user.UserType != UserType.Admin)
            {
                db.UserTicketLinks.Find(user.UserID, ticketID).LastViewed = DateTime.Now;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool postCommentOnTicket(TicketComment comment)
        {
            var user = getCurrentUser();
            comment.UserID = user.UserID;
            if (comment.Text != null && !comment.Text.Trim().Equals("") && (db.UserTicketLinks.Find(user.UserID, comment.TicketID) != null || user.UserType != UserType.Customer))
            {
                comment.User = user;
                db.TicketComments.Add(comment);
                db.SaveChanges();
            }
            if (user.UserType != UserType.Admin)
            {
                db.UserTicketLinks.Find(user.UserID, comment.TicketID).LastViewed = DateTime.Now;
                db.Tickets.Find(comment.TicketID).LastCommentDate = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        public bool changeTicketState(int id, string newState)
        {
            var user = getCurrentUser();
            if (user.UserType == UserType.Customer)
            {
                return false;
            }
            if (user.UserTicketLinks.Where(utl => utl.TicketID == id).Count() != 1 || user.UserType == UserType.Admin)
            {
                return false;
            }
            var ticket = getTicket(id);
            ticket.TicketState = (TicketState)(Enum.Parse(typeof(TicketState), newState));
            db.Entry(ticket).State = System.Data.EntityState.Modified;
            var te = new TicketEvent();
            te.NewTicketState = ticket.TicketState;
            te.text = "Ticket " + ticket.TicketID + " ( " + ticket.TicketTitle + " ) was changed to state " + ticket.TicketState + " by " + user.Email;
            te.TicketID = id;
            te.Ticket = ticket;
            db.TicketEvents.Add(te);
            if (user.UserType != UserType.Admin)
            {
                db.UserTicketLinks.Find(user.UserID, id).LastViewed = DateTime.Now;
                ticket.LastCommentDate = DateTime.Now;
            }
            db.SaveChanges();
            return true;
        }

        public bool assignUserToTicket(int userID, int ticketID)
        {
            var user = getCurrentUser();
            if (user.UserID == userID)
            {
                return false;
            }
            if (user.UserType == UserType.Customer)
            {
                return false;
            }
            var assignedToUser = getUser(userID);
            if (assignedToUser.UserType != UserType.Help)
            {
                return false;
            }
            var ticket = getTicket(ticketID);
            if (!assignedToUser.UserTicketLinks.Select(utl => utl.Ticket).Contains(ticket))
            {
                var userTicketLink = new UserTicketLink();
                userTicketLink.UserID = assignedToUser.UserID;
                userTicketLink.User = assignedToUser;
                userTicketLink.TicketID = ticket.TicketID;
                userTicketLink.Ticket = ticket;
                db.UserTicketLinks.Add(userTicketLink);
                ticket.TicketState = TicketState.Open;

                var ticketEvent = new TicketEvent();
                ticketEvent.NewTicketState = TicketState.Open;
                ticketEvent.text = "Ticket " + ticket.TicketID + " ( " + ticket.TicketTitle + " ) assigned to " + assignedToUser.Email + " by " + user.Email;
                ticketEvent.TicketID = ticket.TicketID;
                ticketEvent.Ticket = ticket;
                db.TicketEvents.Add(ticketEvent);
                db.SaveChanges();
            }
            return true;
        }


        //private methods
        private User getCurrentUser()
        {
            if (!WebSecurity.IsAuthenticated)
            {
                return null;
            }
            return db.Users.Find(WebSecurity.CurrentUserId);
        }

        private int getCurrentUserId()
        {
            return (WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserId : -1);
        }

        private User getUser(int id)
        {
            return db.Users.Find(id);
        }

        private bool getCurrentUserIsAdmin()
        {
            var user = getCurrentUser();
            return (user != null && (user.UserType == UserType.Admin || user.UserType == UserType.Service));
        }

        private List<ShoppingCartItem> getCurrentUsersShoppingCartItems()
        {
            return getCurrentUser().ShoppingCartItems.ToList();
        }

        private Product getProduct(int id)
        {
            return db.Products.Find(id);
        }

        public Ticket getTicket(int id)
        {
            return db.Tickets.Find(id);
        }
    }
}