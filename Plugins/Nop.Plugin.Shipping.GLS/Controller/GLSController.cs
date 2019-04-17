using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.GLS.Models;
using Nop.Plugin.Shipping.GLS.Services;
using Nop.Plugin.Shipping.GLS.Settings;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.GLS.Controller
{
    public class GLSController : BaseAdminController
    {
        private readonly ILogger _logger;
        private readonly GLSSettings _glsSettings;
        private readonly ISettingService _settingService;
        private readonly IGLSService _glsService;

        public GLSController(ILogger logger, GLSSettings glsSettings, ISettingService settingService, IGLSService glsService)
        {
            this._logger = logger;
            this._glsSettings = glsSettings;
            this._settingService = settingService;
            this._glsService = glsService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            GLSModel model = null;
            try
            {
                model = GetBaseModel();

            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure GLS: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }

            return await Task.Run(() => View("~/Plugins/Shipping.GLS/Views/Configure.cshtml", model));
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(GLSModel model)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                _glsSettings.EndpointAddress = model.EndpointAddress;
                _glsSettings.AmountNearestShops = model.AmountNearestShops;                
                _glsSettings.Tracing = model.Tracing;
                _glsSettings.PricesEndsWith = model.PricesEndsWith;
                _glsSettings.FreeShippingLimit = model.FreeShippingLimit;

                _settingService.SaveSetting(_glsSettings);                

                _glsService.UpdateCountries(model.GLSCountries);                
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure GLS: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }

            return View("~/Plugins/Shipping.GLS/Views/Configure.cshtml", model);
        }

        private GLSModel GetBaseModel()
        {
            return new GLSModel
            {
                EndpointAddress = _glsSettings.EndpointAddress,
                AmountNearestShops = _glsSettings.AmountNearestShops,
                GLSCountries = _glsService.GetAllCountries(),
                Tracing = _glsSettings.Tracing,
                PricesEndsWith = _glsSettings.PricesEndsWith,
                FreeShippingLimit = _glsSettings.FreeShippingLimit
            };
        }
    }
}
