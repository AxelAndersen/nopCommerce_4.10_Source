using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.POS.Kaching.Models;
using Nop.Services.Configuration;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Controller
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class POSKachingController : BaseAdminController
    {
        private readonly POSKachingSettings _kachingSettings;
        private readonly ISettingService _settingService;

        public POSKachingController(POSKachingSettings kachingSettings, ISettingService settingService)
        {
            _kachingSettings = kachingSettings;
            _settingService = settingService;
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
    }
}
