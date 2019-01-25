using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.GLS.Settings
{
    public class GLSSettings : ISettings
    {
        public string EndpointAddress { get; set; }

        public int AmountNearestShops { get; set; }

        public decimal SwedishRate { get; set; }

        public bool Tracing { get; set; }
    }
}
