using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalSuppliers.STM
{
    public class STMSettings : ISettings
    {
        public string EndpointAddress { get; set; }

        public string MinimumStockCount { get; set; }      
    }
}