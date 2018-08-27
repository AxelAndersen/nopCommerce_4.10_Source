
using Nop.Plugin.ExternalSuppliers.STM.Extensions;
using Nop.Plugin.ExternalSuppliers.STM.Models;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Nop.Plugin.ExternalSuppliers.STM.Components
{
    public class STMSchedule : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly STMSettings _stmSettings;
        private List<VariantData> _variantData;
        private int _newlyCreatedProducts = 0, _newlyCreatePSC = 0, _updatedPSCs = 0, _newActivePSCs = 0, _newRemovedPSCs = 0, _leftOutDueToBrand = 0, _leftoutDueToStockType = 0, _leftOutDueToMissingEAN = 0, _leftoutDueToLowStock = 0, _leftOutDueToMissingSizeOrColor = 0;
        private string _leaveOutBrands = ";tentipi;", _extraInfo;

        public STMSchedule(ILogger logger, STMSettings stmSettings)
        {
            this._logger = logger;
            this._stmSettings = stmSettings;            
        }

        public async void Execute()
        {
            try
            {
                ValidateSettings();

                await GetVariantData();

                SaveVariantData();

            }
            catch (Exception ex)
            {
                _logger.Error("STMSchedule.Execute()", ex);
            }            
        }

        private async System.Threading.Tasks.Task GetVariantData()
        {
            XElement xElementResult = null;
            using (STMService.STM_APISoapClient client = new STMService.STM_APISoapClient(STMService.STM_APISoapClient.EndpointConfiguration.STM_APISoap12, _stmSettings.EndpointAddress))
            {
                // Both timeouts are needed (the call takes about 3 minutes)
                client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 25, 00); // 25 minutes                    
                client.InnerChannel.OperationTimeout = new TimeSpan(0, 25, 00); // 25 minutes

                xElementResult = await client.GetItemsAsync();
            }

            VariantData data;
            _variantData = new List<VariantData>();
            foreach (XmlNode node in xElementResult.ToXmlDocument().DocumentElement.ChildNodes)
            {
                data = new VariantData();
                data.OrgItemNumber = data.SupplierProductId = node["ITEMNUMBER"].InnerText;
                data.Brand = node["BRAND"].InnerText;
                data.SetSupplierProductId();

                data.OriginalTitle = data.Title = node["ITEMNAME"].InnerText.Trim();
                data.StockCount = Convert.ToInt32(node["INVENTORY"].InnerText.Trim());
                data.RetailPrice = Math.Round(Convert.ToDecimal(node["SALESPRICE"].InnerText.Trim(), CultureInfo.InvariantCulture), 2);
                data.EAN = node["BARCODE"].InnerText.Trim().Replace("-", "");

                data.SizeStr = (node["SIZE"].InnerText.Trim().ToLower() == "stk." || node["SIZE"].InnerText.Trim().ToLower() == "-") ? "" : node["SIZE"].InnerText.Trim();
                data.SetSizeString();

                data.ColorStr = node["COLOR"].InnerText.Trim();

                _variantData.Add(data);
            }
        }

        private void SaveVariantData()
        {
            Console.WriteLine("Saving data to database, updating stock count");
            int count = 0;
            foreach (VariantData data in _variantData)
            {
                if (_leaveOutBrands.Contains(";" + data.Brand.ToLower().Trim() + ";"))
                {
                    _leftOutDueToBrand++;
                    continue;
                }

                if (string.IsNullOrEmpty(data.EAN) || data.EAN.Length < 10)
                {
                    _leftOutDueToMissingEAN++;
                    continue;
                }

                if (data.MissingSizeOrColor())
                {
                    _leftOutDueToMissingSizeOrColor++;
                    continue;
                }

                SaveVariant(data);

                count++;                
            }
        }

        private void SaveVariant(VariantData data)
        {
            throw new NotImplementedException();
        }

        private void ValidateSettings()
        {
            if(this._stmSettings == null)
            {
                throw new Exception("No STM settings found, aborting task");
            }

            if (string.IsNullOrEmpty(this._stmSettings.EndpointAddress))
            {
                throw new Exception("No EndpointAddress found in STM settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._stmSettings.MinimumStockCount))
            {
                throw new Exception("No MinimumStockCount found in STM settings, aborting task");
            }          
        }
    }
}
