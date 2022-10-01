using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanonStore.Models
{
    public class ItemCart
    {
        public Product Producti { get; set; }

        public int Quantity { get; set; }

        public float LineTotal { get; set; }
    }
}