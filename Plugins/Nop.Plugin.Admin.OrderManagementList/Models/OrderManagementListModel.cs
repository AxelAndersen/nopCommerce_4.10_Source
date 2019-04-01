using Nop.Plugin.Admin.OrderManagementList.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Models
{
    public class OrderManagementListModel
    {
        public int TotalCount { get; set; }

        public decimal TotalAmount { get; set; }

        public string ErrorMessage { get; set; }

        public int MarkedProductId { get; set; }

        public string SearchPhrase { get; set; }

        public List<AOPresentationOrder> PresentationOrders { get; set; }
    }
}
