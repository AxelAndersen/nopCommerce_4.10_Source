using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Admin.OrderManagementList.Data;
using Nop.Plugin.Admin.OrderManagementList.Domain;
using Nop.Plugin.Admin.OrderManagementList.Models;
using Nop.Services.Catalog;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public class ReOrderService : IReOrderService
    {
        private readonly OrderManagementContext _context;
        private readonly IManufacturerService _manufacturerService;
        private readonly IVendorService _vendorService;
        private List<Vendor> _allVendors;
        private List<Manufacturer> _allManufacturers;

        public ReOrderService(OrderManagementContext context, IManufacturerService manufacturerService, IVendorService vendorService)
        {
            this._context = context;
            this._allManufacturers = manufacturerService.GetAllManufacturers().ToList();
            this._allVendors = vendorService.GetAllVendors().ToList();
        }

        public List<PresentationReOrderItem> GetCurrentReOrderList(ref int markedProductId, string searchphrase = "")
        {
            List<AOReOrderItem> reOrders = _context.AOReOrderItems.OrderBy(o => o.VendorId).ThenBy(o => o.ManufacturerId).ThenBy(o => o.ManufacturerProductId).ToList();

            List<PresentationReOrderItem> presentationReOrderItems = new List<PresentationReOrderItem>();
            foreach (AOReOrderItem item in reOrders)
            {
                presentationReOrderItems.Add(new PresentationReOrderItem()
                {
                    Id = item.Id,
                    OrderItemId = item.OrderItemId,
                    ManufacturerProductId = item.ManufacturerProductId,
                    ProductId = item.ProductId,
                    EAN = item.EAN,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Manufacturer = GetManufacturer(item.ManufacturerId),
                    Vendor = GetVendor(item.VendorId)
                }
                );
            }

            return presentationReOrderItems;
        }

        private string GetVendor(int vendorId)
        {
            string vendorName = string.Empty;
            try
            {
                Vendor vendor = _allVendors.Where(v => v.Id == vendorId).FirstOrDefault();
                if (vendor == null)
                {
                    vendorName = "Vendor not found (Id: " + vendorId + ")";
                }
                else
                {
                    vendorName = vendor.Name + " (" + vendorId + ")";
                }
            }
            catch (Exception ex)
            {
                vendorName = ex.Message;
            }
            return vendorName;
        }

        private string GetManufacturer(int manufacturerId)
        {
            string manufacturerName = string.Empty;
            try
            {
                Manufacturer manufacturer = _allManufacturers.Where(m => m.Id == manufacturerId).FirstOrDefault();
                if (manufacturer == null)
                {
                    manufacturerName = "Manufacturer not found (Id: " + manufacturerId + ")";
                }
                else
                {
                    manufacturerName = manufacturer.Name + " (" + manufacturerId + ")";
                }
            }
            catch (Exception ex)
            {
                manufacturerName = ex.Message;
            }
            return manufacturerName;
        }

        public int CountDown(int reOrderItemId)
        {
            AOReOrderItem reOrderItem = _context.AOReOrderItems.Where(r => r.Id == reOrderItemId).FirstOrDefault();
            if(reOrderItem == null)
            {
                throw new ArgumentException("No AOReOrderItem found with id: " + reOrderItemId);
            }

            reOrderItem.Quantity -= 1;
            _context.Update(reOrderItem);
            _context.SaveChanges();

            return reOrderItem.Quantity;
        }
    }
}
