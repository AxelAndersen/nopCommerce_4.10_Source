using System.Collections.Generic;
using Nop.Plugin.Admin.OrderManagementList.Domain;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public interface IReOrderService
    {
        List<AOReOrderItem> GetCurrentReOrderList(ref int markedProductId, string searchphrase = "");
    }
}