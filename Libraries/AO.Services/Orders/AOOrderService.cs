using AO.Services.DatabaseContext;
using AO.Services.Orders.Models;
using Microsoft.Extensions.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AO.Services.Orders
{
    public class AOOrderService : IAOOrderService
    {
        //private readonly IDbContext _context;
        private readonly OrderMangementContext _orderContext;
        private readonly IConfiguration _configuration;

        public AOOrderService(IConfiguration configuration)
        {
            //this._context = context;
            this._configuration = configuration;
            this._orderContext = new OrderMangementContext(_configuration);
        }

        public List<AOPresentationOrder> GetCurrentOrders(bool onlyReadyToShip = false)
        {            
            var orders = _orderContext.AOOrders;

            List<AOPresentationOrder> presentationOrders = orders
                                .Select(x => new AOPresentationOrder()
                                {
                                    OrderId = x.OrderId,
                                    CustomerComment = x.CustomerComment,
                                    CustomerEmail = x.CustomerEmail,
                                    CustomerInfo = x.CustomerInfo,
                                    InternalComment = x.InternalComment,
                                    OrderDateTime = x.OrderDateTime,
                                    ShippingInfo = x.ShippingInfo,
                                    TotalOrderAmount = x.TotalOrderAmount,
                                    PresentationOrderItems = GetProductInfo(x)
                                })
                                .ToList();
            
            return presentationOrders;
        }

        /// <summary>
        /// Returns a list of string arrays. Each array contains 2 items, 1 with id, 1 with text
        /// </summary>
        private List<string[]> GetProductInfo(AOOrder order)
        {
            List<string[]> productInfo = new List<string[]>();
            if (string.IsNullOrEmpty(order.OrderItems))
            {
                productInfo.Add(NoOrderItem("No order items found for this order"));
                return productInfo;
            }

            var orderItems = order.OrderItems.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in orderItems)
            {
                productInfo.Add(BuildOrderItem(item));
            }

            return productInfo;
        }

        private string[] BuildOrderItem(string item)
        {
            string[] lineItems = item.Split(':');
            if (lineItems.Length < 2)
            {
                return NoOrderItem("lineItems.Length wrong: " + lineItems.Length);
            }

            string colorSizeText = GetAttributeInfo(lineItems[1]);
            string[] info = new string[2];
            info[0] = lineItems[0];
            info[1] = colorSizeText;
            return info;
        }

        private string GetAttributeInfo(string attributeXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(attributeXml);
            List<int> ids = new List<int>();

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                ids.Add(Convert.ToInt32(node.ChildNodes[0].InnerText));
            }

            List<AOProductAttributeValue> attributes = _orderContext.AOProductAttributeValues(ids.ToArray());
            string res = attributes.Select(a => a.Name).Aggregate(
                "", // start with empty string to handle empty list case.
                     (current, next) => current + ", " + next);

            return res;
        }

        private static string[] NoOrderItem(string message)
        {
            string[] item = new string[2];
            item[0] = "0";
            item[1] = message;
            return item;
        }
    }
}