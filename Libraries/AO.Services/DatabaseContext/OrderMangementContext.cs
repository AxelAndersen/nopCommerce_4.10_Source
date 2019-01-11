using AO.Services.Orders.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace AO.Services.DatabaseContext
{
    public class OrderMangementContext : DbContext
    {
        private DbSet<AOOrder> Orders { get; set; }
        private DbSet<Product> Products { get; set; }
        private DbSet<AOProductAttributeValue> ProductAttributeValues { get; set; }

        private readonly IConfiguration _configuration;

        public OrderMangementContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<AOOrder> AOOrders
        {
            get
            {
                return Orders.OrderByDescending(o => o.Id).ToList();
            }
        }

        public List<Product> AOProducts
        {
            get
            {
                return Products.ToList();
            }
        }

        public List<AOProductAttributeValue> AOProductAttributeValues(int[] ids)
        {
            return ProductAttributeValues.Where(p => ids.Contains(p.Id)).ToList();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AOOrder>().ToTable("OrderManagementList");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<AOProductAttributeValue>().ToTable("AOProductAttributeValue");            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            optionsBuilder.UseSqlServer(dataSettings.DataConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
