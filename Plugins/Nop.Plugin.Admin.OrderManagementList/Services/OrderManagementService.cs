using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Admin.OrderManagementList.Data;
using Nop.Plugin.Admin.OrderManagementList.Domain;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
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
        private readonly IOrderService _orderService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private Shipment _shipment;
        private List<AOPresentationOrder> _presentationOrders;

        public OrderManagementService(IRepository<Order> aoOrderRepository, 
                                      OrderManagementContext context, 
                                      ILogger logger, 
                                      IProductAttributeService productAttributeService, 
                                      IWorkContext workContext, 
                                      IShipmentService shipmentService, 
                                      IOrderService orderService, 
                                      IWorkflowMessageService workflowMessageService, 
                                      IRepository<ProductAttributeCombination> productAttributeCombinationRepository)
        {
            this._logger = logger;
            this._aoOrderRepository = aoOrderRepository;
            this._context = context;
            this._productAttributeService = productAttributeService;
            this._workContext = workContext;
            this._workikngCultureInfo = new CultureInfo(_workContext.WorkingLanguage.UniqueSeoCode);
            this._shipmentService = shipmentService;
            this._orderService = orderService;
            this._workflowMessageService = workflowMessageService;
            this._productAttributeCombinationRepository = productAttributeCombinationRepository;
        }

        #region Public methods
        public List<AOPresentationOrder> GetCurrentOrders(ref int markedProductId, string searchphrase = "")
        {
            _presentationOrders = GetOrders();
            if (string.IsNullOrEmpty(searchphrase))
            {
                return _presentationOrders;
            }

            long num = 0;
            bool isNumber = long.TryParse(searchphrase, out num);
            if (isNumber)
            {
                if(searchphrase.Length <= 8)
                {
                    _presentationOrders = _presentationOrders.Where(o => o.OrderId == num).ToList();
                }
                else
                {
                    ProductAttributeCombination productAttributeCombination = GetProductAttributeCombinationByGtin(searchphrase);
                    if (productAttributeCombination != null)
                    {
                        markedProductId = productAttributeCombination.ProductId;
                        _presentationOrders = _presentationOrders.Where(o => o.PresentationOrderItems.Any(p => p.ProductId == productAttributeCombination.ProductId)).ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                _presentationOrders = _presentationOrders
                    .Where(o => o.PresentationOrderItems
                    .Any(p => p.ProductName.ToLower().Contains(searchphrase.ToLower())))
                    .ToList();
            }

            return _presentationOrders;
        }

        public void SetProductIsTakenAside(int orderItemId, int productId, bool isTakenAside, ref string errorMessage)
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

        public void SetProductOrdered(int orderItemId, int productId, bool isOrdered, ref string errorMessage)
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

        public void SetTrackingNumberOnShipment(string shipmentStr, string trackingNumber)
        {
            int shipmentId = GetShipmentId(shipmentStr);
            _shipment = _shipmentService.GetShipmentById(shipmentId);

            if (_shipment == null)
            {
                throw new ArgumentException("No Shipment found with shipmentId: " + shipmentId);
            }

            _shipment.TrackingNumber = trackingNumber;
            _shipmentService.UpdateShipment(_shipment);
        }

        public void SendShipmentMail(AOOrder order)
        {
            if(_shipment == null)
            {
                int shipmentId = GetShipmentId(order.Shipment);
                _shipment = _shipmentService.GetShipmentById(shipmentId);
            }

            _workflowMessageService.SendShipmentSentCustomerNotification(_shipment, _workContext.WorkingLanguage.Id);
        }  

        public void ChangeOrderStatus(int orderId)
        {
            Order order = _orderService.GetOrderById(orderId);
            if(order == null)
            {
                throw new ArgumentException("No order found with id: " + orderId);
            }

            order.OrderStatusId = (int)OrderStatus.Complete;

            _orderService.UpdateOrder(order);
        }

        /// <summary>
        /// Gets a product attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product attribute combination</returns>
        public virtual ProductAttributeCombination GetProductAttributeCombinationByGtin(string ean)
        {
            if (string.IsNullOrEmpty(ean))
                return null;

            ean = ean.Trim();

            var query = from pac in _productAttributeCombinationRepository.Table
                        orderby pac.Id
                        where pac.Gtin == ean
                        select pac;
            var combination = query.FirstOrDefault();
            return combination;
        }

        public string GetTotalAmount()
        {
            decimal totalAmount = 0;
            foreach (AOPresentationOrder o in _presentationOrders)
            {
                totalAmount += o.TotalOrderAmount;
            }
            return totalAmount.ToString("N", _workikngCultureInfo) + " " + _workContext.WorkingCurrency.CurrencyCode;
        }
        #endregion

        #region Private methods
        private List<AOPresentationOrder> GetOrders()
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
                TotalOrderAmountStr = GetTotal(order),
                TotalOrderAmount = order.TotalOrderAmount,                
                PresentationOrderItems = GetProductInfo(order),
                FormattedPaymentStatus = GetPaymentStatus(order.PaymentStatusId)
            }).ToList();

            return presentationOrders;
        }

        private static int GetShipmentId(string shipmentStr)
        {
            if (shipmentStr.Contains(";") == false)
            {
                throw new ArgumentException("Shipment string missing shipmentId: '" + shipmentStr + "'");
            }

            int shipmentId = 0;
            bool ok = int.TryParse(shipmentStr.Substring(0, shipmentStr.IndexOf(";")), out shipmentId);
            if (ok == false || shipmentId == 0)
            {
                throw new ArgumentException("Shipment string missing proper shipmentId: '" + shipmentStr + "'");
            }

            return shipmentId;
        }

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
            string ship = order.Shipment;

            if (string.IsNullOrEmpty(ship) == false)
            {                
                if (ship.Contains(";"))
                {
                    shippingInfo += "<br /><br />Admin comment:<br />";
                    shippingInfo += ship.Substring(ship.IndexOf(";") + 1);
                }
            }
            
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

            var items = orderItemsStr.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in items)
            {
                var itemContent = s.Split(';');
                orderItems.Add(new AOOrderItem()
                {
                    ProductId = Convert.ToInt32(itemContent[0]),
                    OrderItemId = Convert.ToInt32(itemContent[1]),
                    ProductName = GetProductName(itemContent), 
                    IstakenAside = itemContent.Length > 4 ? ( itemContent[4] == "1" ? true : false) : false,
                    IsOrdered = itemContent.Length > 5 ? (itemContent[5] == "1" ? true : false) : false,
                    Quantity = itemContent.Length > 6 ? (Convert.ToInt32(itemContent[6])) : 0
                });
            }

            return orderItems;
        }

        private string GetProductName(string[] itemContent)
        {
            string productName = itemContent[2].ToString();
            if (itemContent.Length > 3)
            {
                productName += GetAttributeInfo(itemContent[3].ToString());
            }
            
            if(itemContent.Length > 6)
            {
                productName += " <span class='spnQuantity'>(" + itemContent[6] + " stk.)</span>";
            }

            return productName;
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
                        formattedStatus = string.Format(mask, "paymentstatus-green", "Betalt");
                        break;
                    }
                case PaymentStatus.Paid:
                    {
                        formattedStatus = string.Format(mask, "paymentstatus-green", "Betalt og captured");
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