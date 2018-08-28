using AO.Services.Products;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.ExternalSuppliers.Intersurf.Models;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Components
{
    public class IntersurfSchedule : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly IntersurfSettings _intersurfSettings;
        private readonly IProductAttributeService _productAttributeService;
        private readonly string _destinationPath;
        private readonly IAOProductService _aoProductService;
        private readonly IProductService _productService;
        private List<VariantData> _variantData;
        private List<string> _usedEans = new List<string>();

        public IntersurfSchedule(ILogger logger, IntersurfSettings intersurfSettings, IAOProductService aoProductService, IProductAttributeService productAttributeService, IProductService productService)
        {
            this._logger = logger;
            this._intersurfSettings = intersurfSettings;
            this._destinationPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + _intersurfSettings.CSVFileName;
            this._aoProductService = aoProductService;
            this._productAttributeService = productAttributeService;
            this._productService = productService;
        }

        public async void Execute()
        {            
            try
            {
                // Validates that every setting is set correct in the Configure area.
                ValidateSettings();


                // TESTING, incomment again when running for real
                // Gets the data content from asmx service and saves in csv file
                //await GetCSVContentFromAPI();

                // Add data to VariantData list
                ReadDataFromFile();


                SaveVariantData();
            }
            catch (Exception ex)
            {
                _logger.Error("IntersurfSchedule.Execute()", ex);                
            }            
        }

        private void SaveVariantData()
        {            
            int count = 0;
            foreach (VariantData data in _variantData)
            {
                try
                {
                    if (_usedEans.Contains(data.EAN))
                    {
                        
                    }
                    else
                    {
                        _usedEans.Add(data.EAN);
                        SaveProductVariant(data);
                    }
                    count++;
                }
                catch (Exception ex)
                {
                    _logger.Error("IntersurfSchedule.Execute()", ex);                    
                }                
            }
        }

        private void SaveProductVariant(VariantData data)
        {
            var productAttributeCombination = _aoProductService.GetProductAttributeCombinationByGtin(data.EAN);
            if(productAttributeCombination == null)
            {
                return;
            }

            productAttributeCombination.StockQuantity = data.StockCount;
            _productAttributeService.UpdateProductAttributeCombination(productAttributeCombination);
        }

        private async System.Threading.Tasks.Task GetCSVContentFromAPI()
        {
            IntersurfSR.GetCSVStringResponse csvContent = null;

            using (IntersurfSR.CSV_ServiceSoapClient client = new IntersurfSR.CSV_ServiceSoapClient(IntersurfSR.CSV_ServiceSoapClient.EndpointConfiguration.CSV_ServiceSoap12, _intersurfSettings.EndpointAddress))
            {
                var serviceHeader = new IntersurfSR.SecuredWebServiceHeader();
                serviceHeader.Username = _intersurfSettings.Username; // "60750600";
                serviceHeader.Password = _intersurfSettings.Password; // "friliv";

                IntersurfSR.AuthenticateUserResponse response = await client.AuthenticateUserAsync(serviceHeader);
                serviceHeader.AuthenticatedToken = response.AuthenticateUserResult;

                // Both timeouts are needed (the call takes about 3 minutes)
                client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 25, 00); // 25 minutes                    
                client.InnerChannel.OperationTimeout = new TimeSpan(0, 25, 00); // 25 minutes

                csvContent = await client.GetCSVStringAsync(serviceHeader);
            }

            System.IO.File.WriteAllText(_destinationPath, csvContent.GetCSVStringResult.Replace(System.Environment.NewLine, ""));
        }

        private void ReadDataFromFile()
        {
            _variantData = File.ReadAllLines(@"" + _destinationPath)
               .Skip(1)
               .Select(t => VariantData.FromCsv(t, _logger))
               .ToList();           
        }

        private void ValidateSettings()
        {
            if(this._intersurfSettings == null)
            {
                throw new Exception("No Intersurf settings found, aborting task");
            }

            if (string.IsNullOrEmpty(this._intersurfSettings.EndpointAddress))
            {
                throw new Exception("No EndpointAddress found in Intersurf settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._intersurfSettings.Username))
            {
                throw new Exception("No Username found in Intersurf settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._intersurfSettings.Password))
            {
                throw new Exception("No Password found in Intersurf settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._intersurfSettings.CSVFileName))
            {
                throw new Exception("No CSVFileName found in Intersurf settings, aborting task");
            }

            if (this._intersurfSettings.CSVFileName.EndsWith(".csv") == false)
            {
                throw new Exception("CSVFileName must end with '.csv', aborting task");
            }

        }
    }
}
