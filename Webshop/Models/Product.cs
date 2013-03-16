﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
    }
}