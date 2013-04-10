using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public abstract class WebshopDAO : DbContext
    {
        public abstract DbSet<Product> getProducts();
        public abstract DbSet<ProductProperty> getProductProperties();
        public abstract DbSet<User> getUsers();
        public abstract DbSet<Address> getAddresses();
        public abstract DbSet<ShoppingCartItem> getShoppingCartItems();
        public abstract DbSet<Order> getOrders();
        public abstract DbSet<OrderItem> getOrderItems();
        public abstract DbSet<Ticket> getTickets();
        public abstract DbSet<TicketComment> getTicketComments();
        public abstract DbSet<TicketEvent> getTicketEvents();
        public abstract DbSet<UserTicketLink> getUserTicketLinks();
    }
}