using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.ExternalSuppliers.Intersurf
{
    public class IntersurfSettings : ISettings
    {
        public string EndpointAddress { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string CSVFileName { get; set; }
    }
}