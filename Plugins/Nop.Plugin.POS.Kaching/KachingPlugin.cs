using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;
using System;

namespace Nop.Plugin.POS.Kaching
{
    public class KachingPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;

        public KachingPlugin(IWebHelper webHelper)
        {
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        /// 
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/POSKaching/Configure";
        }
    }
}