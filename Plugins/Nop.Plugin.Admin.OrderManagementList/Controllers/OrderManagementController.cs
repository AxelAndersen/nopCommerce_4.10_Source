using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Admin.OrderManagementList.Domain;
using Nop.Plugin.Admin.OrderManagementList.Models;
using Nop.Plugin.Admin.OrderManagementList.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
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
        private readonly IOrderManagementService _orderManagementService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public OrderManagementController(ILogger logger, OrderManagementSettings orderManagementSettings, ILocalizationService localizationService, ISettingService settingService, IOrderManagementService orderManagementService)
        {
            this._logger = logger;
            this._orderManagementSettings = orderManagementSettings;
            this._settingService = settingService;
            this._orderManagementService = orderManagementService;
            this._localizationService = localizationService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult List()
        {
            OrderManagementListModel model = new OrderManagementListModel();

            try
            {
                model.PresentationOrders = _orderManagementService.GetCurrentOrdersAsync();                
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
        public IActionResult UpdateProductReady(int orderId, int orderItemId, int productId, bool isReady)
        {
            if (isReady)
            {
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Admin.OrderManagementList.SuccessfullProductReady"));
                string result = DoUpdateProductReady(orderId, orderItemId, productId, isReady);
                return Json("Produkt klar, " + result);
            }
            else
            {
                string result = DoUpdateProductReady(orderId, orderItemId, productId, isReady);
                return Json("Produkt IKKE klar, " + result);
            }
        }

        private string DoUpdateProductReady(int orderId, int orderItemId, int productId, bool isReady)
        {
            string errorMessage = "";
            bool allwell = false; // _aoOrderService.SetProductIsReady(orderId, orderItemId, productId, isReady);
            if (allwell && string.IsNullOrEmpty(errorMessage))
            {                
                return _localizationService.GetResource("Nop.Plugin.Admin.OrderManagementList.SuccessUpdate");
            }
            else
            {
                return _localizationService.GetResource("Nop.Plugin.Admin.OrderManagementList.FailedUpdate: " + errorMessage);
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
