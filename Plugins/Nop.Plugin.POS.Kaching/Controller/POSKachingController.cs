using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.POS.Kaching.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;

namespace Nop.Plugin.POS.Kaching.Controller
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class POSKachingController : BaseAdminController
    {
        private readonly ILogger _logger;
        private readonly POSKachingSettings _kachingSettings;
        private readonly ISettingService _settingService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;

        public POSKachingController(ILogger logger, POSKachingSettings kachingSettings, ISettingService settingService, IPictureService pictureService, IProductAttributeService productAttributeService, IProductService productService)
        {
            this._logger = logger;
            this._kachingSettings = kachingSettings;
            this._settingService = settingService;
            this._productService = productService;
            this._pictureService = pictureService;
            this._productAttributeService = productAttributeService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var model = new KachingConfigurationModel
            {
                POSKaChingHost = _kachingSettings.POSKaChingHost,
                POSKaChingId = _kachingSettings.POSKaChingId,
                POSKaChingAccountToken = _kachingSettings.POSKaChingAccountToken,
                POSKaChingAPIToken = _kachingSettings.POSKaChingAPIToken,
                POSKaChingImportQueueName = _kachingSettings.POSKaChingImportQueueName
            };

            return View("~/Plugins/Nop.Plugin.POS.Kaching/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(KachingConfigurationModel model)
        {            
            if (!ModelState.IsValid)
                return Configure();

            _kachingSettings.POSKaChingHost = model.POSKaChingHost;
            _kachingSettings.POSKaChingId = model.POSKaChingId;
            _kachingSettings.POSKaChingAccountToken = model.POSKaChingAccountToken;
            _kachingSettings.POSKaChingAPIToken = model.POSKaChingAPIToken;
            _kachingSettings.POSKaChingImportQueueName = model.POSKaChingImportQueueName;
            
            _settingService.SaveSetting(_kachingSettings);

            return Configure();
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult SendAllProducts()
        {
            IPagedList<Core.Domain.Catalog.Product> products = _productService.SearchProducts();

            foreach(Core.Domain.Catalog.Product product in products)
            {
                try
                {
                    POSKachingService service = new POSKachingService(_kachingSettings, _pictureService, _productAttributeService);
                    var json = service.BuildJSONString(product);
                    
                    service.SaveProduct(json);
                }
                catch (Exception ex)
                {
                    _logger.Error("HandleEvent POS Kaching", ex);
                }
            }

            return Configure();
        }        
    }
}
