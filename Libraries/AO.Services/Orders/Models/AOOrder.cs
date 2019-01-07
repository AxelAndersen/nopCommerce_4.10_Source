﻿using Nop.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace AO.Services.Orders.Models
{
    public class AOOrder : BaseEntity
    {
        [Key]
        public int OrderId { get; set; }

        public decimal TotalOrderAmount { get; set; }

        public DateTime OrderDateTime { get; set; }

        public string CustomerInfo { get; set; }

        public string CustomerEmail { get; set; }

        public string ShippingInfo { get; set; }

        public string CustomerComment { get; set; }

        public string InternalComment { get; set; }

        public string OrderItems { get; set; }        
    }
}
