using Microsoft.EntityFrameworkCore;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Admin.OrderManagementList.Data;
using Nop.Plugin.Admin.OrderManagementList.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{

    public class OrderManagementService : IOrderManagementService
    {
        private readonly IRepository<AOOrderManagementAttribute> _aoOrderManagementAttributeRepository;
        private readonly IRepository<Order> _aoOrderRepository;
        private readonly OrderManagementContext _context;


        public OrderManagementService(IRepository<AOOrderManagementAttribute> aoOrderManagementAttributeRepository, IRepository<Order> aoOrderRepository, OrderManagementContext context)
        {
            _aoOrderManagementAttributeRepository = aoOrderManagementAttributeRepository;
            _aoOrderRepository = aoOrderRepository;
            _context = context;
        }

        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Log(AOOrderManagementAttribute att)
        {
            _aoOrderManagementAttributeRepository.Insert(att);
        }

        public List<AOPresentationOrder> GetCurrentOrdersAsync(bool onlyReadyToShip = false)
        {
            var orders = _context.AoOrders.Select(a => new AOOrder()
            {
                Id = a.Id,
                OrderId = a.OrderId,
                TotalOrderAmount = a.TotalOrderAmount,
                Currency = a.Currency,
                OrderDateTime = a.OrderDateTime,
                CustomerInfo = a.CustomerInfo,
                CustomerEmail = a.CustomerEmail,
                ShippingInfo = a.ShippingInfo,
                CheckoutAttributeDescription = a.CheckoutAttributeDescription,
                OrderItems = a.OrderItems,
                OrderNotes = a.OrderNotes
            }
            );

            List<AOPresentationOrder> presentationOrders = orders.Select(order => new AOPresentationOrder()
            {
                OrderId = order.Id,
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
            if (string.IsNullOrEmpty(order.OrderNotes))
            {
                return "&nbsp;";
            }

            var orderNotes = order.OrderNotes.TrimStart(',').Replace(",", "<hr class='hrOrderManagement' /><br />").Replace("#", "<br />");
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
                    ProductName = itemContent[2].ToString() + " " + GetAttributeInfo(itemContent[3].ToString())
                });
            }

            return orderItems;
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

            //List<AOProductAttributeValue> attributes = _orderContext.AOProductAttributeValues(ids.ToArray());
            //string res = attributes.Select(a => a.Name).Aggregate(
            //    "", // start with empty string to handle empty list case.
            //         (current, next) => current + ", " + next);

            return ""; // res;
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
