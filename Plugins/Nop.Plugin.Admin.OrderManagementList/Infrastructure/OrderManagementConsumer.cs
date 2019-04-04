using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList.Infrastructure
{
    public class OrderManagementConsumer : IConsumer<EntityUpdatedEvent<Core.Domain.Orders.Order>>
    {
        private readonly ILogger _logger;
        private readonly IShipmentService _shipmentService;

        public OrderManagementConsumer(ILogger logger, IShipmentService shipmentService)
        {
            this._logger = logger;
            this._shipmentService = shipmentService;
        }

        public void HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            try
            {
                Order order = eventMessage.Entity;
                if (order == null)
                {
                    return;
                }

                AddShipment(order);

                AddToReOrderList(order);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private static void AddToReOrderList(Order order)
        {
            if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
            {

            }
        }

        private void AddShipment(Order order)
        {
            if (order.Shipments == null || order.Shipments.Count <= 0)
            {
                Shipment shipment = new Shipment()
                {
                    OrderId = order.Id,
                    AdminComment = "Shipment added automatically",
                    TotalWeight = 1
                };

                _shipmentService.InsertShipment(shipment);
            }
        }
    }
}
