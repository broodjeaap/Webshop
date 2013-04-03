using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace Webshop.Models
{
    public class WebshopContext : DbContext, WebshopDAO
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketEvent> TicketEvents { get; set; }
        public DbSet<UserTicketLink> UserTicketLinks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public void Seed (WebshopContext db)
        {
            #if DEBUG
                var user = new User();
                user.Email = "david";
                db.Users.Add(user);
                user = new User();
                user.Email = "help1";
                db.Users.Add(user);
                user = new User();
                user.Email = "help2";
                db.Users.Add(user);
                user = new User();
                user.Email = "help3";
                db.Users.Add(user);
                user = new User();
                user.Email = "chrome";
                db.Users.Add(user);
                db.SaveChanges();
                WebSecurity.CreateAccount("david", "password");
                WebSecurity.CreateAccount("help1", "password");
                WebSecurity.CreateAccount("help2", "password");
                WebSecurity.CreateAccount("help3", "password");
                WebSecurity.CreateAccount("chrome", "password");
            #endif
        }

        public class DropCreateIfChangeInitializer : DropCreateDatabaseIfModelChanges<WebshopContext>
        {
            protected override void Seed (WebshopContext context)
            {
                context.Seed (context);

                base.Seed (context);
            }
        }

        public class CreateInitializer : CreateDatabaseIfNotExists<WebshopContext>
        {
            protected override void Seed (WebshopContext context)
            {
                context.Seed (context);

                base.Seed (context);
            }
        }

        static WebshopContext ()
        {
            #if DEBUG
            //Database.SetInitializer<WebshopContext>(new DropCreateIfChangeInitializer());
            #else
            //Database.SetInitializer<WebshopContext> (new CreateInitializer ());
            #endif
        }
    }
}