namespace Nop.Plugin.Admin.OrderManagementList.Models
{
    public class PresentationReOrderItem
    {         
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int OrderItemId { get; set; }

        public string ManufacturerProductId { get; set; }

        public int ManufacturerId { get; set; }

        public string ManufacturerName { get; set; }

        public string VendorName { get; set; }

        public int VendorId { get; set; }

        public string VendorEmail { get; set; }

        public string EAN { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }        
    }
}
