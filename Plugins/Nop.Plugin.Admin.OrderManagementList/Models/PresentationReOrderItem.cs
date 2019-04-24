namespace Nop.Plugin.Admin.OrderManagementList.Models
{
    public class PresentationReOrderItem
    {         
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int OrderItemId { get; set; }

        public string ManufacturerProductId { get; set; }

        public string Manufacturer { get; set; }

        public string Vendor { get; set; }

        public string EAN { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }        
    }
}
