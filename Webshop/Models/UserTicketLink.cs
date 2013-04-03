using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public class UserTicketLink
    {
        public UserTicketLink()
        {
            LastViewed = DateTime.Now;
        }

        [Key, Column(Order = 0)]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        [Key, Column(Order = 1)]
        public int TicketID { get; set; }
        public virtual Ticket Ticket { get; set; }

        public DateTime LastViewed { get; set; }
    }
}