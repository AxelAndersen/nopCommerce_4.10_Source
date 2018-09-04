using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.ExternalSuppliers.STM.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalSuppliers.STM.Controller
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class STMController : BaseAdminController
    {
        private readonly ILogger _logger;
        private readonly STMSettings _stmSettings;
        private readonly ISettingService _settingService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;

        public STMController(ILogger logger, STMSettings stmSettings, ISettingService settingService, IPictureService pictureService, IProductAttributeService productAttributeService, IProductService productService)
        {
            this._logger = logger;
            this._stmSettings = stmSettings;
            this._settingService = settingService;
            this._productService = productService;
            this._pictureService = pictureService;
            this._productAttributeService = productAttributeService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            STMModel model = null;
            try
            {
                model = GetBaseModel();

            }        
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure STM: " + inner.Message, ex);                
                model.ErrorMessage += "<br />" + inner.Message;
            }

            return await Task.Run(() => View("~/Plugins/ExternalSuppliers.STM/Views/Configure.cshtml", model));            
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(STMModel model)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                _stmSettings.EndpointAddress = model.EndpointAddress;
                _stmSettings.MinimumStockCount = model.MinimumStockCount;           

                _settingService.SaveSetting(_stmSettings);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure STM: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }

            return View("~/Plugins/ExternalSuppliers.STM/Views/Configure.cshtml", model);
        }               

        private STMModel GetBaseModel()
        {
            return new STMModel
            {
                EndpointAddress = _stmSettings.EndpointAddress,
                MinimumStockCount = _stmSettings.MinimumStockCount
            };
        }
    }
}
