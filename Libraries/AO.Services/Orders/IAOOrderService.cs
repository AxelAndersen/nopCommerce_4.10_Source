using System.Collections.Generic;
using AO.Services.Orders.Models;

namespace AO.Services.Orders
{
    public interface IAOOrderService
    {
        List<AOOrder> GetCurrentOrders(bool onlyReadyToShip = false);
    }
}