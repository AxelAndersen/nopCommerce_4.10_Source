using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System;
using System.Linq;

namespace Nop.Plugin.Admin.OrderManagementList
{
    public class OrderManagementPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public OrderManagementPlugin(IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService)
        {
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Admin.OrderManagementList",
                Title = "Order management",
                ControllerName = "OrderManagement",
                ActionName = "List",
                Visible = true,
                IconClass = "fa fa-dot-circle-o", 
                Url = $"{_webHelper.GetStoreLocation()}Admin/OrderManagement/List.cshtml",

            };
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Sales");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        /// 
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/OrderManagement/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new OrderManagementSettings());

            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.ListActive", "Active");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.WelcomeMessage", "Welcome message");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.ErrorMessage", "Error message");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<OrderManagementSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.ListActive");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.WelcomeMessage");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.ErrorMessage");

            base.Uninstall();
        }
    }
}
