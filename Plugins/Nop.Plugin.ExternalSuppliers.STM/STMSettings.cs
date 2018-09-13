using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalSuppliers.STM
{
    public class STMSettings : ISettings
    {
        public string EndpointAddress { get; set; }

        public string MinimumStockCount { get; set; }

        /// <summary>
        /// This is to know how far in the list we have reached.
        /// We cant take everything at once, due to timeouts.
        /// </summary>
        public int SkipNumber { get; set; }        
    }
}