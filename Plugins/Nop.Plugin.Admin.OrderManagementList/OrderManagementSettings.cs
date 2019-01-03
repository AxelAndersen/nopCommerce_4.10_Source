using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList
{
    public class OrderManagementSettings : ISettings
    {
        public bool ListActive { get; set; }
        public string WelcomeMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}
