using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Admin.OrderManagementList.Models;
using Nop.Plugin.Admin.OrderManagementList.Services;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Controllers
{
    public class ReOrderController : BaseAdminController
    {
        private readonly ILogger _logger;
        private readonly IReOrderService _reOrderService;

        public ReOrderController(ILogger logger, IReOrderService reOrderService)
        {
            this._logger = logger;
            this._reOrderService = reOrderService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult ReOrderList(string searchphrase = "")
        {
            PresentationReOrderListModel model = new PresentationReOrderListModel();

            try
            {
                int markedProductId = 0, totalCount = 0;
                model.ReOrderItems = _reOrderService.GetCurrentReOrderList(ref markedProductId, ref totalCount, searchphrase);
                model.SearchPhrase = searchphrase;
                model.TotalCount = totalCount;
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("List Order Management: " + inner.Message, ex);
                model.ErrorMessage = ex.ToString();
            }

            return View("~/Plugins/Nop.Plugin.Admin.OrderManagementList/Views/ReOrderList.cshtml", model);
        }

        [HttpGet]
        [AuthorizeAdmin(false)]
        public IActionResult ReOrderItemChangeQuantity(int reOrderItemId, int quantity)
        {
            int newQuantity = _reOrderService.ChangeQuantity(reOrderItemId, quantity);
            return Json(newQuantity);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult ReOrderNote(int vendorId, string vendorName)
        {
            ReOrderNoteModel model = new ReOrderNoteModel();

            try
            {
                int markedProductId = 0, totalcount = 0;
                model.ReOrderItems = _reOrderService.GetCurrentReOrderList(ref markedProductId, ref totalcount, "", vendorId);
                model.VendorName = vendorName;
                model.TotalCount = GetProductsCount(model.ReOrderItems);
                model.CompleteVendorEmail = _reOrderService.GetCompleteVendorEmail(model.ReOrderItems, vendorId);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("List Order Management: " + inner.Message, ex);
                model.ErrorMessage = ex.ToString();
            }

            return View("~/Plugins/Nop.Plugin.Admin.OrderManagementList/Views/ReOrderNote.cshtml", model);
        }

        private int GetProductsCount(List<PresentationReOrderItem> reOrderItems)
        {
            int totalProductsCount = 0;
            foreach(PresentationReOrderItem item in reOrderItems)
            {
                totalProductsCount += item.Quantity;
            }
            return totalProductsCount;
        }
    }
}