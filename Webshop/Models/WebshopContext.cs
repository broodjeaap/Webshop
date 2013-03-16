using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public class WebshopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}