using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System;

namespace Nop.Plugin.POS.Kaching
{
    public class KachingPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public KachingPlugin(IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService)
        {
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._localizationService = localizationService;
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

            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.POS.Kaching.KaChingHost", "Kaching host");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.POS.Kaching.POSKaChingId", "Kaching Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.POS.Kaching.POSKaChingAccountToken", "Kaching Account Token");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.POS.Kaching.POSKaChingAPIToken", "Kaching API Token");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.POS.Kaching.POSKaChingImportQueueName", "Kaching ImportQueue name");            


            base.Install();
        }
    }
}