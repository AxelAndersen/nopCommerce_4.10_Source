﻿using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Admin.OrderManagementList.Models;
using Nop.Plugin.Admin.OrderManagementList.Services;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;

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
            ReOrderListModel model = new ReOrderListModel();

            try
            {
                int markedProductId = 0;
                model.ReOrderItems = _reOrderService.GetCurrentReOrderList(ref markedProductId, searchphrase);
                model.MarkedProductId = markedProductId;
                model.SearchPhrase = searchphrase;
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
    }
}