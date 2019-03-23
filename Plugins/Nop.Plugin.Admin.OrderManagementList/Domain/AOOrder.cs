﻿using Nop.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Admin.OrderManagementList.Domain
{
    public class AOOrder : BaseEntity
    {
        [Key]
        public virtual int OrderId { get; set; }

        public virtual decimal TotalOrderAmount { get; set; }

        public virtual string Currency { get; set; }

        public virtual DateTime OrderDateTime { get; set; }

        public virtual string CustomerInfo { get; set; }

        public virtual string CustomerEmail { get; set; }

        public virtual string ShippingInfo { get; set; }

        public virtual string CheckoutAttributeDescription { get; set; }

        public virtual string OrderItems { get; set; }

        public virtual string OrderNotes { get; set; }

        public virtual int PaymentStatusId { get; set; }
    }
}