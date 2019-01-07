using AO.Services.Orders.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;

namespace AO.Services.DatabaseContext
{
    public class OrderMangementContext : DbContext
    {
        private DbSet<AOOrder> Orders { get; set; }
        private DbSet<Product> Products { get; set; }

        private readonly IConfiguration _configuration;

        public OrderMangementContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<AOOrder> AOOrders
        {
            get
            {
                return Orders.ToList();
            }
        }

        public List<Product> AOProducts
        {
            get
            {
                return Products.ToList();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AOOrder>().ToTable("OrderManagementList");
            modelBuilder.Entity<Product>().ToTable("Product");
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
