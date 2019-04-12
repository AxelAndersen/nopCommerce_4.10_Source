using Nop.Plugin.Admin.OrderManagementList.Data;
using Nop.Plugin.Admin.OrderManagementList.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public class ReOrderService : IReOrderService
    {
        private readonly OrderManagementContext _context;

        public ReOrderService(OrderManagementContext context)
        {
            this._context = context;
        }

        public List<AOReOrderItem> GetCurrentReOrderList(ref int markedProductId, string searchphrase = "")
        {
            List<AOReOrderItem> reOrders = _context.AOReOrderItems.ToList();

            return reOrders;
        }
    }
}
