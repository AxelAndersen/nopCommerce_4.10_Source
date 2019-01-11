using AO.Services.Orders;
using AO.Services.Orders.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Admin.OrderManagementList.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class OrderManagementController : BaseAdminController
    {
        private readonly ILogger _logger;
        private readonly OrderManagementSettings _orderManagementSettings;
        private readonly IAOOrderService _aoOrderService;
        private readonly ISettingService _settingService;

        public OrderManagementController(ILogger logger, OrderManagementSettings orderManagementSettings, ISettingService settingService, IAOOrderService aoOrderService)
        {
            this._logger = logger;
            this._orderManagementSettings = orderManagementSettings;
            this._settingService = settingService;
            this._aoOrderService = aoOrderService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult List()
        {
            OrderManagementListModel model = new OrderManagementListModel();

            try
            {
                var orders  = _aoOrderService.GetCurrentOrders();

                model.PresentationOrders = new List<AOPresentationOrder>();

                foreach (AOPresentationOrder order in orders)
                {
                    model.PresentationOrders.Add(new AOPresentationOrder()
                    {
                        OrderId = order.OrderId,
                        CustomerComment = order.CustomerComment,
                        CustomerEmail = order.CustomerEmail,
                        CustomerInfo = order.CustomerInfo,                        
                        OrderNotes = order.OrderNotes,
                        OrderDateTime = order.OrderDateTime,
                        PresentationOrderItems = order.PresentationOrderItems,
                        ShippingInfo = order.ShippingInfo,
                        TotalOrderAmount = order.TotalOrderAmount
                    }
                    );                 
                }
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("List Order Management: " + inner.Message, ex);
                model.ErrorMessage = ex.ToString();
            }

            return View("~/Plugins/Nop.Plugin.Admin.OrderManagementList/Views/List.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            OrderManagementConfigurationModel model = null;
            try
            {
                model = GetBaseModel();
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure Order Management: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }
            return View("~/Plugins/Nop.Plugin.Admin.OrderManagementList/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(OrderManagementConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            try
            {
                _orderManagementSettings.ListActive = model.ListActive;
                _orderManagementSettings.WelcomeMessage = model.WelcomeMessage;
                _orderManagementSettings.ErrorMessage = model.ErrorMessage;

                _settingService.SaveSetting(_orderManagementSettings);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure Order Management: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }
            return Configure();
        }

        [HttpGet]
        [AuthorizeAdmin(false)]
        public IActionResult UpdateProductReady(int productId, bool ready)
        {
            if (ready)
            {
                string result = DoUpdateProductReady(productId, ready);
                return Json("Produkt klar, " + result);
            }
            else
            {
                string result = DoUpdateProductReady(productId, ready);
                return Json("Produkt IKKE klar, " + result);
            }
        }

        private string DoUpdateProductReady(int productId, bool ready)
        {
            string errorMessage = "";
            bool allwell = _aoOrderService.UpdateReadyOrNot(productId, ready, ref errorMessage);
            if (allwell && string.IsNullOrEmpty(errorMessage))
            {
                return "Opdatering lykkes";
            }
            else
            {
                return "Opdatering mislykkes";
            }
        }

        private OrderManagementConfigurationModel GetBaseModel()
        {
            return new OrderManagementConfigurationModel
            {
                ListActive = _orderManagementSettings.ListActive,
                WelcomeMessage = _orderManagementSettings.WelcomeMessage,
                ErrorMessage = _orderManagementSettings.ErrorMessage
            };
        }
    }
}
