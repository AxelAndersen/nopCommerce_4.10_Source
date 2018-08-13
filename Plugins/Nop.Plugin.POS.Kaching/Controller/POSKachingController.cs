using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.POS.Kaching.Models;
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
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var model = new KachingConfigurationModel
            {
                POSKaChingHost = "sss",
                POSKaChingId = "dd"
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
            
            return Configure();
        }
    }
}
