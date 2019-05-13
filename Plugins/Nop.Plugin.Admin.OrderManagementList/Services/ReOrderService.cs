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

        public List<PresentationReOrderItem> GetCurrentReOrderList(ref int markedProductId, ref int totalCount, string searchphrase = "", int vendorId = 0)
        {
            List<AOReOrderItem> reOrders = null;

            if (vendorId == 0)
            {
                reOrders = _context.AOReOrderItems
                            .Where(o => o.Quantity > 0)
                            .OrderBy(o => o.VendorId)
                            .ThenBy(o => o.ManufacturerId)
                            .ThenBy(o => o.ManufacturerProductId)
                            .ToList();
            }
            else
            {
                reOrders = _context.AOReOrderItems
                            .Where(o => o.Quantity > 0 && o.VendorId == vendorId)
                            .OrderBy(o => o.VendorId)
                            .ThenBy(o => o.ManufacturerId)
                            .ThenBy(o => o.ManufacturerProductId)
                            .ToList();
            }

            totalCount = reOrders.Count;
            int currentVendorId = 0;
            List<PresentationReOrderItem> presentationReOrderItems = new List<PresentationReOrderItem>();
            foreach (AOReOrderItem item in reOrders)
            {
                Manufacturer manufacturer = GetManufacturer(item.ManufacturerId);
                Vendor vendor = GetVendor(item.VendorId);

                if (vendorId == 0) // This is only for ReOrderList with all vendors.
                {
                    if (currentVendorId != item.VendorId)
                    {
                        currentVendorId = item.VendorId;

                        // Add space with vendor details
                        presentationReOrderItems.Add(new PresentationReOrderItem()
                        {
                            Id = 0,
                            EAN = "",
                            ManufacturerId = manufacturer.Id,
                            ManufacturerName = manufacturer.Name,
                            ManufacturerProductId = "",
                            VendorName = vendor.Name,
                            VendorId = vendor.Id,
                            VendorEmail = vendor.Email,
                            OrderItemId = 0,
                            ProductId = 0,
                            ProductName = "Spacing",
                            Quantity = 0
                        });
                    }
                }

                presentationReOrderItems.Add(new PresentationReOrderItem()
                {
                    Id = item.Id,
                    OrderItemId = item.OrderItemId,
                    ManufacturerProductId = GetManufacturerProductId(item),
                    ProductId = item.ProductId,
                    EAN = item.EAN,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    ManufacturerId = manufacturer.Id,
                    ManufacturerName = manufacturer.Name,
                    VendorName = vendor.Name,
                    VendorId = vendor.Id,
                }
                );
            }

            if (string.IsNullOrEmpty(searchphrase) == false)
            {
                presentationReOrderItems = presentationReOrderItems
                    .Where(o => o.ProductName.ToLower().Contains(searchphrase.ToLower())
                    || o.VendorName.ToLower().Contains(searchphrase.ToLower())
                    || o.ManufacturerName.ToLower().Contains(searchphrase.ToLower())
                    || o.ManufacturerProductId.ToLower().Contains(searchphrase.ToLower()))
                    .ToList();
            }

            return presentationReOrderItems;
        }

        private string GetManufacturerProductId(AOReOrderItem item)
        {
            string mId = item.ManufacturerProductId;

            if (mId.Contains("-FrilivId:("))
            {
                mId = mId.Substring(0, mId.IndexOf("-FrilivId:("));
            }

            if (string.IsNullOrEmpty(item.EAN) == false)
            {
                mId += "&nbsp;&nbsp;&nbsp;EAN: " + item.EAN;
            }

            return mId;
        }

        public int ChangeQuantity(int reOrderItemId, int quantity)
        {
            AOReOrderItem reOrderItem = _context.AOReOrderItems.Where(r => r.Id == reOrderItemId).FirstOrDefault();
            if (reOrderItem == null)
            {
                throw new ArgumentException("No AOReOrderItem found with id: " + reOrderItemId);
            }

            reOrderItem.Quantity += quantity; // When negative we decrease
            _context.Update(reOrderItem);
            _context.SaveChanges();

            return reOrderItem.Quantity;
        }

        public void RemoveFromReOrderList(int quantityToOrder, int orderItemId)
        {
            AOReOrderItem reOrderItem = _context.AOReOrderItems.Where(r => r.OrderItemId == orderItemId).FirstOrDefault();
            if (reOrderItem == null)
            {
                throw new ArgumentException("Intet fundet på bestillingslisten");
            }

            if (quantityToOrder > reOrderItem.Quantity)
            {
                // If we wanna reduce with more than is on reorderlist.
                // This can happen if we had some in stock on order time
                // But we only have the total quantity from the orderitem here.
                quantityToOrder = reOrderItem.Quantity;
            }

            reOrderItem.Quantity -= quantityToOrder;
            reOrderItem.OrderedQuantity = quantityToOrder;
            _context.AOReOrderItems.Update(reOrderItem);
            _context.SaveChanges();
        }

        public void ReAddToReOrderList(int orderItemId)
        {
            AOReOrderItem reOrderItem = _context.AOReOrderItems.Where(r => r.OrderItemId == orderItemId).FirstOrDefault();
            if (reOrderItem == null)
            {
                throw new ArgumentException("Intet fundet på bestillingslisten");
            }

            reOrderItem.Quantity = (reOrderItem.OrderedQuantity.HasValue && reOrderItem.OrderedQuantity.Value > 0) ? reOrderItem.OrderedQuantity.Value : 1;
            reOrderItem.OrderedQuantity = 0;
            _context.AOReOrderItems.Update(reOrderItem);
            _context.SaveChanges();
        }

        public string GetCompleteVendorEmail(List<PresentationReOrderItem> reOrderItems, int vendorId)
        {
            Vendor vendor = GetVendor(vendorId);
            if(vendor == null || vendor.Id == 0)
            {
                throw new ArgumentException("No vendor found with id: " + vendorId);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("mailto:");
            sb.Append(vendor.Email);
            sb.Append("?subject=Bestilling af varer&");
            sb.Append("body=Hej%0D%0A%0D%0AHermed bestilling på følgende:%0D%0A%0D%0A%0D%0A");
            foreach (PresentationReOrderItem item in reOrderItems)
            {
                sb.Append(item.Quantity.ToString() + "%20stk.%20");
                sb.Append(item.ManufacturerName + "%20%20%20");
                sb.Append(item.ManufacturerProductId.Replace("&nbsp;", "%20") + "%20%20%20");                
                sb.Append(item.ProductName);                
                sb.Append("%0D%0A");
                sb.Append("----------------------------------------------------------------------------------------------------------------------------------");
                sb.Append("%0D%0A");
            }
            sb.Append("%0D%0A%0D%0A");
            sb.Append("Med venlig hilsen");
         
            sb.Append("%0D%0AFriliv.dk"); // friliv.dk
            sb.Append("%0D%0AJuelstrupparken 26"); // Juelstrupparken 26
            sb.Append("%0D%0A9530 Støvring"); // 9533 Støvring

            return sb.ToString();
        }

        private Vendor GetVendor(int vendorId)
        {
            Vendor vendor = null;
            try
            {
                vendor = _allVendors.Where(v => v.Id == vendorId).FirstOrDefault();
                if (vendor == null)
                {
                    vendor = new Vendor() { Name = "Vendor not found (Id: " + vendorId + ")" };
                }
            }
            catch (Exception ex)
            {
                vendor = new Vendor() { Name = ex.Message };
            }
            return vendor;
        }

        private Manufacturer GetManufacturer(int manufacturerId)
        {
            Manufacturer manufacturer = null;
            try
            {
                manufacturer = _allManufacturers.Where(m => m.Id == manufacturerId).FirstOrDefault();
                if (manufacturer == null)
                {
                    manufacturer = new Manufacturer() { Name = "Manufacturer not found (Id: " + manufacturerId + ")" };
                }
            }
            catch (Exception ex)
            {
                manufacturer = new Manufacturer() { Name = ex.Message };
            }

            return manufacturer;
        }
    }
}