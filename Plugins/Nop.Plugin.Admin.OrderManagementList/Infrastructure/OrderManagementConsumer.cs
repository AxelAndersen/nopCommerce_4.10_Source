using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Plugin.Admin.OrderManagementList.Data;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Nop.Plugin.Admin.OrderManagementList.Infrastructure
{
    public class OrderManagementConsumer : IConsumer<EntityUpdatedEvent<Core.Domain.Orders.Order>>
    {
        private readonly ILogger _logger;
        private readonly IShipmentService _shipmentService;
        private readonly OrderManagementContext _context;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private int _handledOrderItems = 0;


        public OrderManagementConsumer(ILogger logger, 
                                       OrderManagementContext context, 
                                       IShipmentService shipmentService,
                                       IProductAttributeService productAttributeService,
                                       IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
                                       IProductService productService,
                                       IManufacturerService manufacturerService)
        {
            this._logger = logger;
            this._context = context;
            this._shipmentService = shipmentService;
            this._productAttributeService = productAttributeService;
            this._productAttributeCombinationRepository = productAttributeCombinationRepository;
            this._productService = productService;
            this._manufacturerService = manufacturerService;
        }

        public void HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            try
            {
                Order order = eventMessage.Entity;
                if (order == null)
                {
                    _logger.Error("OrderManagementConsumer HandleEvent, Order is null");
                    return;
                }

                AddShipment(order);

                AddToReOrderList(order);

                ValidateReordering(order);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("OrderManagementConsumer HandleEvent: " + inner.Message, ex);                
            }
        }

        private void ValidateReordering(Order order)
        {
            if(_handledOrderItems != order.OrderItems.Count)
            {
                throw new Exception("Wrong number of reordering items: " + _handledOrderItems + " should have been " + order.OrderItems.Count);
            }
        }

        #region Private methods
        private void AddToReOrderList(Order order)
        {
            if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Pending) // Pending is only for testing. should be Paid
            {
                _handledOrderItems = 0;
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    var reOrderItem = _context.AOReOrderItems.Where(o => o.OrderItemId == orderItem.Id).FirstOrDefault();
                    if (reOrderItem != null)
                    {
                        _handledOrderItems++;
                        // We already added this item to reorder list
                        continue;
                    }

                    List<ProductAttributeCombination> combinations = GetProductAttributeCombinationsByProductId(orderItem.ProductId);
                    if (combinations == null || combinations.Count == 0)
                    {
                        throw new ArgumentException("No ProductAttributeCombination found with productid " + orderItem.ProductId);
                    }

                    Product product = _productService.GetProductById(orderItem.ProductId);
                    if (product == null)
                    {
                        throw new ArgumentException("No product found with productid " + orderItem.ProductId);
                    }

                    AddReOrderItem(orderItem, reOrderItem, combinations, product);
                }
            }
        }

        private void AddReOrderItem(OrderItem orderItem, Domain.AOReOrderItem reOrderItem, List<ProductAttributeCombination> combinations, Product product)
        {
            string attributeInfo = GetAttributeInfo(orderItem.AttributesXml);
            foreach (ProductAttributeCombination comb in combinations)
            {
                AddItemIfMatch(orderItem, reOrderItem, product, attributeInfo, comb);
            }
        }

        private void AddItemIfMatch(OrderItem orderItem, Domain.AOReOrderItem reOrderItem, Product product, string attributeInfo, ProductAttributeCombination comb)
        {
            string attributeInfoComb = GetAttributeInfo(comb.AttributesXml);
            if (attributeInfoComb == attributeInfo)
            {
                int manufacturerId = 0;
                var manufacturer = _manufacturerService.GetProductManufacturersByProductId(product.Id).FirstOrDefault();
                if (manufacturer != null)
                {
                    manufacturerId = manufacturer.ManufacturerId;
                }

                if (string.IsNullOrEmpty(comb.Gtin) == false)
                {
                    reOrderItem = _context.AOReOrderItems.Where(o => o.EAN == comb.Gtin).FirstOrDefault();
                }
                else
                {
                    reOrderItem = _context.AOReOrderItems.Where(o => o.ManufacturerProductId == product.ManufacturerPartNumber && o.ManufacturerId == manufacturerId).FirstOrDefault();
                }

                AddOrUpdateItem(orderItem, reOrderItem, product, attributeInfo, comb, manufacturerId);
            }
        }

        private void AddOrUpdateItem(OrderItem orderItem, Domain.AOReOrderItem reOrderItem, Product product, string attributeInfo, ProductAttributeCombination comb, int manufacturerId)
        {
            if (reOrderItem == null)
            {
                AddItem(orderItem, product, attributeInfo, comb, manufacturerId);
            }
            else
            {
                UpdateItem(orderItem, reOrderItem);
            }
            _handledOrderItems++;
        }

        private void UpdateItem(OrderItem orderItem, Domain.AOReOrderItem reOrderItem)
        {
            reOrderItem.Quantity += orderItem.Quantity;
            _context.AOReOrderItems.Update(reOrderItem);
            _context.SaveChanges();
        }

        private void AddItem(OrderItem orderItem, Product product, string attributeInfo, ProductAttributeCombination comb, int manufacturerId)
        {
            Domain.AOReOrderItem reOrderItem = new Domain.AOReOrderItem()
            {
                EAN = comb.Gtin ?? "",
                ManufacturerId = manufacturerId,
                ManufacturerProductId = product.ManufacturerPartNumber,
                OrderItemId = orderItem.Id,
                ProductId = comb.ProductId,
                ProductName = product.Name + attributeInfo,
                Quantity = orderItem.Quantity,
                VendorId = product.VendorId
            };
            _context.AOReOrderItems.Add(reOrderItem);
            _context.SaveChanges();            
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

        /// <summary>
        /// Gets a product attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product attribute combination</returns>
        private List<ProductAttributeCombination> GetProductAttributeCombinationsByProductId(int productId)
        {
            var query = from pac in _productAttributeCombinationRepository.Table
                        orderby pac.Id
                        where pac.ProductId == productId
                        select pac;
            var combinations = query.ToList();
            return combinations;
        }

        private void AddShipment(Order order)
        {
            if (order.Shipments == null || order.Shipments.Count <= 0)
            {
                Shipment shipment = new Shipment()
                {
                    OrderId = order.Id,
                    AdminComment = "Shipment added automatically",
                    TotalWeight = 1,
                    CreatedOnUtc = DateTime.Now
                };

                _shipmentService.InsertShipment(shipment);
            }
        } 
        #endregion
    }
}
