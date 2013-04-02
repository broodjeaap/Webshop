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
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketEvent> TicketEvents { get; set; }
    }
}