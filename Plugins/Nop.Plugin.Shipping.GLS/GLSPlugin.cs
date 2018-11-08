using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.GLS.Settings;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Shipping.GLS
{
    public class GLSPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public GLSPlugin(IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService)
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
            return $"{_webHelper.GetStoreLocation()}Admin/GLS/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new GLSSettings());

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<GLSSettings>();

            base.Uninstall();
        }
    }
}
