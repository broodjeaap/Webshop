using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public class TicketEvent
    {
        public TicketEvent()
        {
            EventTime = DateTime.UtcNow;
            NewTicketState = TicketState.New;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TicketEventID { get; set; }
        public DateTime EventTime { get; set; }
        public string text { get; set; }
        public int TicketID { get; set; }
        public Ticket Ticket { get; set; }
        public TicketState NewTicketState { get; set; }
    }
}