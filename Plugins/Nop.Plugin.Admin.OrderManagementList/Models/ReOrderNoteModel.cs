using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList.Models
{
    public class ReOrderNoteModel
    {
        public string ErrorMessage { get; set; }

        public int TotalCount { get; set; }

        public string VendorName { get; set; }

        public string CompleteVendorEmail { get; set; }

        public List<PresentationReOrderItem> ReOrderItems { get; set; }
    }
}
