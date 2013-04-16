using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.SqlServer;
using System.Text;

namespace Webshop.Models
{
    public class Product
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string TextDescription { get; set; }
        public string ImageName { get; set; }
        public string Category { get; set; }
        public string SubCat1 { get; set; }
        public string SubCat2 { get; set; }
        public virtual ICollection<ProductProperty> Properties { get; set; }

        public static string FormatPrice(int price){
            var sb = new StringBuilder();
            sb.Append(price / 100f);
            var str = sb.ToString();
            int pointIndex = str.IndexOf(".");
            if (pointIndex == -1)
            {
                sb.Append(".-");
                return sb.ToString();
            }
            if ((str.Length - pointIndex - 1) == 1)
            {
                sb.Append("0");
                return sb.ToString();
            }
            return sb.ToString();
        }
    }
}