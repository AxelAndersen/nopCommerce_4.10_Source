namespace Nop.Plugin.Admin.OrderManagementList.Domain
{
    public class AOOrderItem
    {
        public int ProductId { get; set; }

        public int OrderItemId { get; set; }

        public string ProductName { get; set; }

        public bool IstakenAside { get; set; }

        public bool IsOrdered { get; set; }        
    }
}
