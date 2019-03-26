using GLSReference;
using Nop.Plugin.Shipping.GLS.Settings;
using Nop.Services.Logging;
using System;
using System.Threading.Tasks;
using static GLSReference.wsShopFinderSoapClient;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public class GLSService : IGLSService
    {
        private readonly GLSSettings _glsSettings;
        private readonly ILogger _logger;

        public GLSService(ILogger logger, GLSSettings glsSettings)
        {
            _logger = logger;
            _glsSettings = glsSettings;
        }

        public PakkeshopData GetParcelShopData(string parcelNumber)
        {
            PakkeshopData parcelShop = null;
            try
            {
                wsShopFinderSoapClient client = new wsShopFinderSoapClient(EndpointConfiguration.wsShopFinderSoap12, _glsSettings.EndpointAddress);
                parcelShop = client.GetOneParcelShopAsync(parcelNumber).Result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            if (parcelShop == null)
            {
                parcelShop = new PakkeshopData();
                parcelShop.Number = "0";
                parcelShop.Streetname = "Fejl ved hentning af pakkeshop data";
            }

            return parcelShop;
        }
    }
}
