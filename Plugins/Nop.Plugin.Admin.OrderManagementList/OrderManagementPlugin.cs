﻿using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
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
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Sales");

            // Add Reorder list            
            var menuItem = new SiteMapNode()
                {
                    SystemName = "ReOrderList",
                    Title = "Bestillingsliste",
                    ControllerName = "ReOrder",
                    ActionName = "ReOrderList",
                    Visible = true,
                    IconClass = "fa fa-dot-circle-o",
                    Url = $"{_webHelper.GetStoreLocation()}Admin/OrderManagement/ReOrderList.cshtml"
                };
            if (pluginNode != null)
                pluginNode.ChildNodes.Insert(0, menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);

            // Add order management list
            menuItem = new SiteMapNode()
            {
                SystemName = "OrderManagement",
                Title = "Ordrehåndtering",
                ControllerName = "OrderManagement",
                ActionName = "List",
                Visible = true,
                IconClass = "fa fa-dot-circle-o", 
                Url = $"{_webHelper.GetStoreLocation()}Admin/OrderManagement/List.cshtml"                
            };
            
            if (pluginNode != null)
                pluginNode.ChildNodes.Insert(0, menuItem);
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
            
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.WelcomeMessage", "Welcome message");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.ErrorMessage", "Error message");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.SuccessUpdate", "Successfull updated");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FailedUpdate", "failed to update");

            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPHost", "FTP Host");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPUsername", "FTP Username");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPPassword", "FTP Password");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPLocalFilePath", "Local filepath");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPLocalFileName", "Local filename");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPRemoteFolderPath", "Remote folderpath");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPPrinterName", "Printer name");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPRemoteStatusFilePath", "Remote status filepath");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPTempFolder", "Temp Folder");

            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.GLSStatusFileRetries", "GLS Status file retries");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.GLSStatusFileWaitSeconds", "GLS Status file wait sec.");

            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DoPrintLabel", "Print label");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DoCapture", "Capture payment from list");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DoSendEmails", "Send email to customer");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.ChangeOrderStatus", "Change order status");

            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DoCleanup", "Do Cleanup");
            _localizationService.AddOrUpdatePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DaysToKeepStatusFiles", "Days to keep Status files");

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
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.WelcomeMessage");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.ErrorMessage");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.SuccessfullProductReady");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FailedUpdate");

            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPHost");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPUsername");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPPassword");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPLocalFilePath");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPLocalFileName");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPRemoteFolderPath");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPPrinterName");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPRemoteStatusFilePath");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.FTPTempFolder");

            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.GLSStatusFileRetries");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.GLSStatusFileWaitSeconds");

            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DoPrintLabel");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DoCapture");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DoSendEmails");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.ChangeOrderStatus");

            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DoCleanup");
            _localizationService.DeletePluginLocaleResource("Nop.Plugin.Admin.OrderManagementList.DaysToKeepStatusFiles");

            base.Uninstall();
        }
    }
}
