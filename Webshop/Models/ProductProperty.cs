using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public class ProductProperty
    {
        public int ProductPropertyID { get; set; }
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
    }
}