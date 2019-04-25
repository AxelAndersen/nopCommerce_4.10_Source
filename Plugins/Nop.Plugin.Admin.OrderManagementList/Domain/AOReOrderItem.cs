using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Admin.OrderManagementList.Domain
{
    public class AOReOrderItem
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int OrderItemId { get; set; }

        public string ManufacturerProductId { get; set; }

        public int ManufacturerId { get; set; }

        public int VendorId { get; set; }

        public string EAN { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public int? OrderedQuantity { get; set; }        
    }
}
