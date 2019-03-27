﻿using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Admin.OrderManagementList.Data;
using Nop.Plugin.Admin.OrderManagementList.Domain;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{

    public class OrderManagementService : IOrderManagementService
    {
        private readonly ILogger _logger;
        private readonly IRepository<Order> _aoOrderRepository;
        private readonly OrderManagementContext _context;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IWorkContext _workContext;
        private readonly CultureInfo _workikngCultureInfo;
        private readonly IShipmentService _shipmentService;

        public OrderManagementService(IRepository<Order> aoOrderRepository, OrderManagementContext context, ILogger logger, IProductAttributeService productAttributeService, IWorkContext workContext, IShipmentService shipmentService)
        {
            this._logger = logger;
            this._aoOrderRepository = aoOrderRepository;
            this._context = context;
            this._productAttributeService = productAttributeService;
            this._workContext = workContext;
            this._workikngCultureInfo = new CultureInfo(_workContext.WorkingLanguage.UniqueSeoCode);
            this._shipmentService = shipmentService;
        }

        #region Public methods
        public List<AOPresentationOrder> GetCurrentOrdersAsync(bool onlyReadyToShip = false)
        {
            List<AOPresentationOrder> presentationOrders = _context.AoOrders.Select(order => new AOPresentationOrder()
            {
                OrderId = order.Id,
                CustomerComment = GetCustomerComment(order),
                CustomerEmail = order.CustomerEmail,
                CustomerInfo = GetCustomerInfo(order),
                OrderNotes = GetOrderNotes(order),
                OrderDateTime = order.OrderDateTime.ToString("dd-MM-yy H:mm"),
                ShippingInfo = GetShippingInfo(order),
                TotalOrderAmount = GetTotal(order),
                PresentationOrderItems = GetProductInfo(order),
                FormattedPaymentStatus = GetPaymentStatus(order.PaymentStatusId)
            }).ToList();

            return presentationOrders;
        }

        public void SetProductIsTakenAside(int orderId, int orderItemId, int productId, bool isTakenAside, ref string errorMessage)
        {
            try
            {
                var orderItemSetting = _context.AOOrderItemSettings.Where(o => o.OrderItemId == orderItemId).FirstOrDefault();
                if (orderItemSetting == null)
                {
                    orderItemSetting = new AOOrderItemSetting()
                    {
                        OrderItemId = orderItemId,
                        IsTakenAside = isTakenAside,
                        IsTakenAsideDate = DateTime.Now,
                        IsOrdered = false,
                        IsOrderedDate = Convert.ToDateTime("01-01-1970")
                    };

                    _context.Add(orderItemSetting);
                }
                else
                {
                    orderItemSetting.IsTakenAside = isTakenAside;
                    orderItemSetting.IsTakenAsideDate = DateTime.Now;
                    _context.Update(orderItemSetting);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error(ex.Message, ex);
            }
        }

        public void SetProductOrdered(int orderId, int orderItemId, int productId, bool isOrdered, ref string errorMessage)
        {
            try
            {
                var orderItemSetting = _context.AOOrderItemSettings.Where(o => o.OrderItemId == orderItemId).FirstOrDefault();
                if (orderItemSetting == null)
                {
                    orderItemSetting = new AOOrderItemSetting()
                    {
                        OrderItemId = orderItemId,
                        IsTakenAside = false,
                        IsTakenAsideDate = Convert.ToDateTime("01-01-1970"),
                        IsOrdered = isOrdered,
                        IsOrderedDate = DateTime.Now
                    };

                    _context.Add(orderItemSetting);
                }
                else
                {
                    orderItemSetting.IsOrdered = isOrdered;
                    orderItemSetting.IsOrderedDate = DateTime.Now;
                    _context.Update(orderItemSetting);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error(ex.Message, ex);
            }
        }

        public AOOrder GetOrder(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("No proper orderId: " + orderId);
            }

            AOOrder order =  _context.AoOrders.Where(o => o.Id == orderId).FirstOrDefault();
            if(order == null)
            {
                throw new ArgumentException("No order found with id: " + orderId);
            }

            return order;
        }

        public void SetTrackingNumberOnShipment(int shipmentId, string trackingNumber)
        {            
            Shipment shipment = _shipmentService.GetShipmentById(shipmentId);

            if (shipment == null)
            {
                throw new ArgumentException("No Shipment found with shipmentId: " + shipmentId);
            }

            shipment.TrackingNumber = trackingNumber;
            _shipmentService.UpdateShipment(shipment);
        }
        #endregion

        #region Private methods
        private string GetCustomerComment(AOOrder order)
        {
            return string.IsNullOrEmpty(order.CheckoutAttributeDescription) ? "&nbsp;" : order.CheckoutAttributeDescription;
        }

        private string GetCustomerInfo(AOOrder order)
        {
            return order.UserName + "<br />" + order.CustomerInfo.Replace("#", "<br />");
        }

        private string GetTotal(AOOrder order)
        {
            return order.TotalOrderAmount.ToString("N2", _workikngCultureInfo) + " " + order.Currency;
        }

        private string GetShippingInfo(AOOrder order)
        {
            if (string.IsNullOrEmpty(order.ShippingInfo))
            {
                return "&nbsp;";
            }

            var shippingInfo = order.ShippingInfo.Replace("#", "<br />");
            return shippingInfo;
        }

        private string GetOrderNotes(AOOrder order)
        {
            if (string.IsNullOrEmpty(order.OrderNotes))
            {
                return "&nbsp;";
            }

            var orderNotes = order.OrderNotes.TrimStart(',').Replace(",", "<hr class='hrSmall' /><br />").Replace("#", "<br />");
            return orderNotes;
        }

        /// <summary>+
        /// Returns a list of string arrays. Each array contains 2 items, 1 with id, 1 with text
        /// </summary>
        private List<AOOrderItem> GetProductInfo(AOOrder order)
        {
            if (string.IsNullOrEmpty(order.OrderItems))
            {
                List<AOOrderItem> errorItems = new List<AOOrderItem>();
                errorItems.Add(new AOOrderItem()
                {
                    ProductId = 0,
                    ProductName = "No order items found for this order"
                });
                return errorItems;
            }

            List<AOOrderItem> orderItems = GetOrderItems(order.OrderItems);
            return orderItems;
        }

        private List<AOOrderItem> GetOrderItems(string orderItemsStr)
        {
            List<AOOrderItem> orderItems = new List<AOOrderItem>();

            var items = orderItemsStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in items)
            {
                var itemContent = s.Split(';');
                orderItems.Add(new AOOrderItem()
                {
                    ProductId = Convert.ToInt32(itemContent[0]),
                    OrderItemId = Convert.ToInt32(itemContent[1]),
                    ProductName = itemContent[2].ToString() + GetAttributeInfo(itemContent[3].ToString()) + " <span class='spnQuantity'>(" + itemContent[6] + " stk.)</span>",
                    IstakenAside = itemContent[4] == "1" ? true : false,
                    IsOrdered = itemContent[5] == "1" ? true : false
                });
            }

            return orderItems;
        }

        private string GetAttributeInfo(string attributeXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(attributeXml);
            string attributeInfo = "";

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                var productAttribute = _productAttributeService.GetProductAttributeValueById(Convert.ToInt32(node.ChildNodes[0].InnerText));

                if (productAttribute != null)
                {
                    attributeInfo += ", " + productAttribute.Name;
                }
            }
            return attributeInfo;
        }

        private static string[] NoOrderItem(string message)
        {
            string[] item = new string[2];
            item[0] = "0";
            item[1] = message;
            return item;
        }

        private string GetPaymentStatus(int paymentStatusId)
        {
            PaymentStatus paymentStatus = (PaymentStatus)paymentStatusId;
            string formattedStatus = "";
            string mask = "<span class='{0}'>{1}</span>";

            switch (paymentStatus)
            {
                case PaymentStatus.Pending:
                    {
                        formattedStatus = string.Format(mask, "paymentstatus-red", "Ikke betalt");
                        break;
                    }
                case PaymentStatus.Authorized:
                    {
                        formattedStatus = string.Format(mask, "paymentstatus-red", "Ikke betalt");
                        break;
                    }
                case PaymentStatus.Paid:
                    {
                        formattedStatus = string.Format(mask, "paymentstatus-green", "Betalt");
                        break;
                    }
                case PaymentStatus.PartiallyRefunded:
                    {
                        formattedStatus = string.Format(mask, "paymentstatus-yellow", "Delvist refunderet");
                        break;
                    }
                case PaymentStatus.Refunded:
                    {
                        formattedStatus = string.Format(mask, "paymentstatus-yellow", "Refunderet");
                        break;
                    }
                case PaymentStatus.Voided:
                    {
                        formattedStatus = string.Format(mask, "paymentstatus-yellow", "Annulleret");
                        break;
                    }
                default:
                    {
                        formattedStatus = string.Format(mask, "paymentstatus-red", "Betaling fejlet");
                        break;
                    }
            }

            return formattedStatus;
        }
        #endregion
    }
}