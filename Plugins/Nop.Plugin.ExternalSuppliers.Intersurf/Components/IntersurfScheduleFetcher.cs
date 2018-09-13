using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Components
{
    public class IntersurfScheduleFetcher : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly IntersurfSettings _intersurfSettings;
        private List<string> _usedEans = new List<string>();
        private readonly string _destinationPath;

        public IntersurfScheduleFetcher(ILogger logger, IntersurfSettings intersurfSettings)
        {
            this._logger = logger;
            this._intersurfSettings = intersurfSettings;
            this._destinationPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + _intersurfSettings.CSVFileName;
        }

        public async void Execute()
        {
            try
            {
                // Validates that every setting is set correct in the Configure area.
                Validation.ValidateSettings(_intersurfSettings);

                // Gets the data content from asmx service and saves in csv file
                // This call runs loanger than the timeout of a WebClient and therefore this HAS to be an async method.
                // This way we return immediately, but we also dispose any services initiated by DI.
                // Thats why we have another schedule to do the product update, using there data.
                // See more here: https://www.nopcommerce.com/boards/t/55357/long-running-scheduled-tasks.aspx
                await GetData();
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("IntersurfScheduleFetcher.Execute(): " + inner.Message, ex);
            }
        }

        private async Task<int> GetData()
        {
            IntersurfSR.GetCSVStringResponse csvContent = null;

            using (IntersurfSR.CSV_ServiceSoapClient client = new IntersurfSR.CSV_ServiceSoapClient(IntersurfSR.CSV_ServiceSoapClient.EndpointConfiguration.CSV_ServiceSoap12, _intersurfSettings.EndpointAddress))
            {
                var serviceHeader = new IntersurfSR.SecuredWebServiceHeader();
                serviceHeader.Username = _intersurfSettings.Username; // "60750600";
                serviceHeader.Password = _intersurfSettings.Password; // "friliv";

                IntersurfSR.AuthenticateUserResponse response = client.AuthenticateUserAsync(serviceHeader).Result;
                serviceHeader.AuthenticatedToken = response.AuthenticateUserResult;

                // Both timeouts are needed (the call takes about 3 minutes)
                client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 25, 00); // 25 minutes                    
                client.InnerChannel.OperationTimeout = new TimeSpan(0, 25, 00); // 25 minutes

                csvContent = await client.GetCSVStringAsync(serviceHeader);
            }

            System.IO.File.WriteAllText(_destinationPath, csvContent.GetCSVStringResult.Replace(System.Environment.NewLine, ""));

            return 1;
        }
    }
}