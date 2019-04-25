using Nop.Plugin.Admin.OrderManagementList.Models;
using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public interface IReOrderService
    {
        List<PresentationReOrderItem> GetCurrentReOrderList(ref int markedProductId, string searchphrase = "");

        int CountDown(int reOrderItemId);

        void RemoveFromReOrderList(int orquantityToOrderderId, int orderItemId);

        void ReAddToReOrderList(int orderItemId);
    }
}