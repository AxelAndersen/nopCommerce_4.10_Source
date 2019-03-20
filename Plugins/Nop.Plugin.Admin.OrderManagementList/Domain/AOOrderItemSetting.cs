using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Admin.OrderManagementList.Domain
{
    public class AOOrderItemSetting
    {
        [Key]
        public int OrderItemId { get; set; }

        public bool IsTakenAside { get; set; }

        public bool IsOrdered { get; set; }

        public DateTime IsTakenAsideDate { get; set; }

        public DateTime IsOrderedDate { get; set; }        
    }
}
