using System;
using System.Collections.Generic;

namespace AO.Services.Orders.Models
{
    public class AOPresentationOrder
    {
        public int OrderId { get; set; }

        public decimal TotalOrderAmount { get; set; }

        public DateTime OrderDateTime { get; set; }

        public string CustomerInfo { get; set; }

        public string CustomerEmail { get; set; }

        public string ShippingInfo { get; set; }

        public string CustomerComment { get; set; }

        public string InternalComment { get; set; }        

        public List<string[]> PresentationOrderItems { get; set; }
    }
}
