using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList.Models
{
    public class OrderManagementConfigurationModel
    {
        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.ListActive")]
        public bool ListActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.WelcomeMessage")]
        public string WelcomeMessage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}
