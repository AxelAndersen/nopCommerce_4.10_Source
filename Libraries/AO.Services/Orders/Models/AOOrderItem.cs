using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AO.Services.Orders.Models
{
    public class AOOrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public string Text { get; set; }

        public string EAN { get; set; }
    }
}
