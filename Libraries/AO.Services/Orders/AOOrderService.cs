using AO.Services.DatabaseContext;
using AO.Services.Orders.Models;
using Microsoft.Extensions.Configuration;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AO.Services.Orders
{
    public class AOOrderService : IAOOrderService
    {        
        private readonly OrderMangementContext _orderContext;
        private readonly IConfiguration _configuration;

        public AOOrderService(IConfiguration configuration)
        {            
            this._configuration = configuration;
            this._orderContext = new OrderMangementContext(_configuration);
        }

        public List<AOPresentationOrder> GetCurrentOrders(bool onlyReadyToShip = false)
        {            
            var orders = _orderContext.AOOrders;

            List<AOPresentationOrder> presentationOrders = orders
                                .Select(order => new AOPresentationOrder()
                                {
                                    OrderId = order.OrderId,
                                    CustomerComment = GetCustomerComment(order),
                                    CustomerEmail = order.CustomerEmail,
                                    CustomerInfo = GetCustomerInfo(order),
                                    OrderNotes = GetOrderNotes(order),
                                    OrderDateTime = order.OrderDateTime.ToString("dd-MM-yy H:mm"),
                                    ShippingInfo = order.ShippingInfo,
                                    TotalOrderAmount = GetTotal(order),
                                    PresentationOrderItems = GetProductInfo(order)
                                })
                                .ToList();
            
            return presentationOrders;
        }

        private string GetCustomerComment(AOOrder order)
        {
            return string.IsNullOrEmpty(order.CheckoutAttributeDescription) ? "&nbsp;" : order.CheckoutAttributeDescription;
        }

        private string GetCustomerInfo(AOOrder order)
        {
            return order.CustomerInfo.Replace("#", "<br />");
        }

        private string GetTotal(AOOrder order)
        {
            return order.TotalOrderAmount.ToString("N2") + " " + order.Currency;
        }

        private string GetOrderNotes(AOOrder order)
        {
            if(string.IsNullOrEmpty(order.OrderNotes))
            {
                return "&nbsp;";
            }
            
            var orderNotes = order.OrderNotes.TrimStart(',').Replace(",", "<hr class='hrOrderManagement' /><br />").Replace("#", "<br />");
            return orderNotes;
        }

        /// <summary>+
        /// Returns a list of string arrays. Each array contains 2 items, 1 with id, 1 with text
        /// </summary>
        private List<Models.OrderItem> GetProductInfo(AOOrder order)
        {
            if (string.IsNullOrEmpty(order.OrderItems))
            {
                List<Models.OrderItem> errorItems = new List<Models.OrderItem>();
                errorItems.Add(new Models.OrderItem()
                {
                    ProductId = 0,
                    ProductName = "No order items found for this order"                
                });
                return errorItems;
            }

            List<Models.OrderItem> orderItems = GetOrderItems(order.OrderItems);           
            return orderItems;
        }

        private List<Models.OrderItem> GetOrderItems(string orderItemsStr)
        {
            List<Models.OrderItem> orderItems = new List<Models.OrderItem>();

            var items = orderItemsStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in items)
            {    
                var itemContent = s.Split(';');
                orderItems.Add(new Models.OrderItem()
                {
                    ProductId = Convert.ToInt32(itemContent[0]),
                    ProductName = itemContent[1].ToString() + " " + GetAttributeInfo(itemContent[2].ToString())
                });   
            }

            return orderItems;
        }

        //private string[] BuildOrderItem(string item)
        //{
        //    string[] lineItems = item.Split(':');
        //    if (lineItems.Length < 2)
        //    {
        //        return NoOrderItem("lineItems.Length wrong: " + lineItems.Length);
        //    }

        //    if (lineItems[0].Contains(";") == false)
        //    {
        //        return NoOrderItem("lineItems[0] does not contain a ';': " + lineItems[0]);
        //    }

        //    string colorSizeText = GetAttributeInfo(lineItems[1]);
        //    string[] info = new string[3];
        //    info[0] = lineItems[0].Substring(0, lineItems[0].IndexOf(";"));
        //    info[1] = lineItems[0].Substring(lineItems[0].IndexOf(";") + 1);
        //    info[2] = colorSizeText;
        //    return info;
        //}

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