using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.GLS.Settings
{
    public class GLSSettings : ISettings
    {
        public string EndpointAddress { get; set; }

        public int AmountNearestShops { get; set; }        

        public bool Tracing { get; set; }

        public string PricesEndsWith { get; set; }

        public decimal FreeShippingLimit { get; set; }

        public string Separator { get; set; }
    }
}
