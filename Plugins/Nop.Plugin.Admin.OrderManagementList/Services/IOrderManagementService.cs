using Nop.Plugin.Admin.OrderManagementList.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public interface IOrderManagementService
    {
        List<AOPresentationOrder> GetCurrentOrdersAsync(bool onlyReadyToShip = false);

        bool SetProductIsTakenAside(int orderId, int orderItemId, int productId, bool isTakenAside);

        bool SetProductOrdered(int orderId, int orderItemId, int productId, bool isOrdered);
    }
}
