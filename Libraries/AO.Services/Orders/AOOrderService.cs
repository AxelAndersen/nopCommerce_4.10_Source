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

        public List<AOOrder> GetCurrentOrders(bool onlyReadyToShip = false)
        {
            List<AOOrder> orders = null;
            try
            {
                var orderContext = new OrderMangementContext(_configuration);
                orders = orderContext.AOOrders;                
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;               
            }
           
            return orders;
        }
    }
}
