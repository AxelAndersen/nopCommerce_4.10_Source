﻿using System;
using System.Collections.Generic;

namespace AO.Services.Orders.Models
{
    public class AOPresentationOrder
    {
        public int OrderId { get; set; }

        public string TotalOrderAmount { get; set; }        

        public string OrderDateTime { get; set; }

        public string CustomerInfo { get; set; }

        public string CustomerEmail { get; set; }

        public string ShippingInfo { get; set; }

        public string CustomerComment { get; set; }

        public string OrderNotes { get; set; }        

        public List<Models.OrderItem> PresentationOrderItems { get; set; }
    }
}
