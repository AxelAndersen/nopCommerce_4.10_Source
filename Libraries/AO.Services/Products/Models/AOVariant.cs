using System;
using System.Collections.Generic;
using System.Text;

namespace AO.Services.Products.Models
{
    public class AOVariant
    {
        public string EAN { get; set; }

        public int ProductId { get; set; }

        public int ColorId { get; set; }

        public string ColorName { get; set; }

        public int SizeId { get; set; }

        public string SizeName { get; set; }
    }
}
