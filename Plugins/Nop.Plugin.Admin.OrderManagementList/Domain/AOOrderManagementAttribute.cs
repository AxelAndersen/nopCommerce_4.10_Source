using Nop.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Admin.OrderManagementList.Domain
{
    public class AOOrderManagementAttribute : BaseEntity
    {
        public virtual int OrderId { get; set; }
        public virtual int OrderItemId { get; set; }
        public virtual int ProductId { get; set; }
        public virtual bool IsReady { get; set; }
        public virtual bool IsOrdered { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}
