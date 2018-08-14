using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using System;

namespace Nop.Plugin.POS.Kaching
{
    public class KachingPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;

        public KachingPlugin(IWebHelper webHelper, ISettingService settingService)
        {
            this._webHelper = webHelper;
            this._settingService = settingService;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        /// 
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/POSKaching/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new POSKachingSettings());

            base.Install();
        }
    }
}