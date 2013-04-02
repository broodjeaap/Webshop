using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public class Ticket
    {
        public Ticket()
        {
            TicketCreationDate = DateTime.UtcNow;
            TicketState = TicketState.New;
            Users = new HashSet<User>();
            TicketComments = new HashSet<TicketComment>();
            TicketEvents = new HashSet<TicketEvent>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TicketID { get; set; }
        public string TicketTitle { get; set; }
        public DateTime TicketCreationDate { get; set; }
        public TicketState TicketState { get; set; }
        public string TicketBody { get; set; }
        public int OwnerUserID { get; set; }
        [ForeignKey("OwnerUserID")]
        public virtual User Owner { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketEvent> TicketEvents { get; set; }
    }
}