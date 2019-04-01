using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Admin.OrderManagementList.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public interface IOrderManagementService
    {
        List<AOPresentationOrder> GetCurrentOrders(ref int markedProductId, string searchphrase = "");

        void SetProductIsTakenAside(int orderId, int orderItemId, int productId, bool isTakenAside, ref string errorMessage);

        void SetProductOrdered(int orderId, int orderItemId, int productId, bool isOrdered, ref string errorMessage);

        AOOrder GetOrder(int orderId);

        void SetTrackingNumberOnShipment(string shipmentStr, string trackingNumber);

        void ChangeOrderStatus(int orderId);

        void SendShipmentMail(AOOrder order);

        ProductAttributeCombination GetProductAttributeCombinationByGtin(string ean);
    }
}