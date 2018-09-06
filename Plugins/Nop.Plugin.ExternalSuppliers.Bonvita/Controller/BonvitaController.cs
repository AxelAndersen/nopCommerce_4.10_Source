using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.ExternalSuppliers.Bonvita.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalSuppliers.Bonvita.Controller
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class BonvitaController : BaseAdminController
    {
        private readonly ILogger _logger;
        private readonly BonvitaSettings _bonvitaSettings;
        private readonly ISettingService _settingService;

        public BonvitaController(ILogger logger, BonvitaSettings bonvitaSettings, ISettingService settingService)
        {
            this._logger = logger;
            this._bonvitaSettings = bonvitaSettings;
            this._settingService = settingService;
        }


        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            BonvitaModel model = null;
            try
            {
                model = GetBaseModel();

            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure Bonvita: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }

            return await Task.Run(() => View("~/Plugins/ExternalSuppliers.Bonvita/Views/Configure.cshtml", model));
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(BonvitaModel model)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                _bonvitaSettings.FtpHost = model.FtpHost;
                _bonvitaSettings.User = model.User;
                _bonvitaSettings.Pass = model.Pass;
                _bonvitaSettings.CSVFileName = model.CSVFileName;

                _settingService.SaveSetting(_bonvitaSettings);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure Bonvita: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }
            return View("~/Plugins/ExternalSuppliers.Bonvita/Views/Configure.cshtml", model);
        }

        private BonvitaModel GetBaseModel()
        {
            return new BonvitaModel
            {
                FtpHost = _bonvitaSettings.FtpHost,
                User = _bonvitaSettings.User,
                Pass = _bonvitaSettings.Pass,
                CSVFileName = _bonvitaSettings.CSVFileName
            };
        }
    }
}
