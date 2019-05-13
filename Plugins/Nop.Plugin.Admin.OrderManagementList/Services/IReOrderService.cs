using Nop.Plugin.Admin.OrderManagementList.Models;
using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public interface IReOrderService
    {
        List<PresentationReOrderItem> GetCurrentReOrderList(ref int markedProductId, ref int totalCount, string searchphrase = "", int vendorId = 0);        

        void RemoveFromReOrderList(int orquantityToOrderderId, int orderItemId);

        void ReAddToReOrderList(int orderItemId);

        int ChangeQuantity(int reOrderItemId, int quantity);

        string GetCompleteVendorEmail(List<PresentationReOrderItem> reOrderItems, int vendorId);
    }
}