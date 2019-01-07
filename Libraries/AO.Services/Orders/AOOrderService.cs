using AO.Services.DatabaseContext;
using AO.Services.Orders.Models;
using Microsoft.Extensions.Configuration;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AO.Services.Orders
{
    public class AOOrderService : IAOOrderService
    {
        private readonly IDbContext _context;
        private readonly IConfiguration _configuration;

        public AOOrderService(IDbContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
        }

        public List<AOPresentationOrder> GetCurrentOrders(bool onlyReadyToShip = false)
        {
            List<AOPresentationOrder> presentationOrders = null;
            try
            {
                var orderContext = new OrderMangementContext(_configuration);
                var orders = orderContext.AOOrders;

                presentationOrders = orders
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
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;                                    
            }
           
            return presentationOrders;
        }

        /// <summary>
        /// Returns a list of string arrays. Each array contains 2 items, 1 with id, 1 with text
        /// </summary>
        private List<string[]> GetProductInfo(AOOrder order)
        {
            List<string[]> productInfo = new List<string[]>();
            if(string.IsNullOrEmpty(order.OrderItems))
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
            if(lineItems.Length < 2)
            {
                return NoOrderItem("lineItems.Length wrong: " + lineItems.Length);
            }
            
            return lineItems;
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
