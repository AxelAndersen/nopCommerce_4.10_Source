using Nop.Plugin.Admin.OrderManagementList.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Models
{
    public class ReOrderListModel
    {
        public int TotalCount { get; set; }

        public string ErrorMessage { get; set; }

        public int MarkedProductId { get; set; }

        public string SearchPhrase { get; set; }

        public List<AOReOrderItem> ReOrderItems { get; set; }
    }
}
