using Nop.Plugin.Admin.OrderManagementList.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public interface IOrderManagementService
    {
        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Log(AOOrderManagementAttribute att);

        List<AOPresentationOrder> GetCurrentOrdersAsync(bool onlyReadyToShip = false);

        bool SetProductIsTakenAside(int orderId, int orderItemId, int productId, bool isTakenAside);
    }
}
