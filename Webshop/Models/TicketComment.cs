using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public class TicketComment
    {
        public TicketComment()
        {
            CommentPostTime = DateTime.Now;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TicketCommentID { get; set; }
        public string Text { get; set; }
        public int TicketID { get; set; }
        public DateTime CommentPostTime { get; set; }
        public virtual Ticket Ticket { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        
    }
}