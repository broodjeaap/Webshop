using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public class User
    {
        public User()
        {
            Addresses = new HashSet<Address>();
            ShoppingCartItems = new HashSet<ShoppingCartItem>();
            Orders = new HashSet<Order>();
            UserType = UserType.Customer;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}