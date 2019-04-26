using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Models
{
    public class PresentationReOrderListModel
    {
        public int TotalCount { get; set; }

        public string ErrorMessage { get; set; }        

        public string SearchPhrase { get; set; }

        public List<PresentationReOrderItem> ReOrderItems { get; set; }
    }
}
