
using System.Linq;
using AO.Services.Extensions;
using AO.Services.Logging;
using AO.Services.Products;
using AO.Services.Products.Models;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.ExternalSuppliers.STM.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Nop.Services.Configuration;

namespace Nop.Plugin.ExternalSuppliers.STM.Components
{
    public class STMSchedule : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly STMSettings _stmSettings;
        private readonly IAOProductService _aoProductService;
        private readonly ISettingService _settingService;
        private List<VariantData> _variantData;
        private XmlNodeList _variantsNodeList;
        private const string _updaterName = "STM";
        private const int _takeNumber = 5000;

        public STMSchedule(ILogger logger, STMSettings stmSettings, ISettingService settingService, IAOProductService aoProductService)
        {
            this._logger = logger;
            this._stmSettings = stmSettings;
            this._aoProductService = aoProductService;
            this._settingService = settingService;
        }

        public void Execute()
        {            
            try
            {
                ValidateSettings();

                GetData();

                OrganizeData();

                _aoProductService.SaveVariantData(_variantData, _updaterName);

                SetSkipNumber();

                var variantText = "";
                if(_variantData[0] != null)
                {
                    variantText = "'" + _variantData[0].Title + " (" + _variantData[0].Brand + ", " + _variantData[0].SupplierProductId + ")'";
                }
                _logger.Information("STM: Done running through " + _variantData.Count + " variants, Started at number " + _stmSettings.SkipNumber + ": " + variantText);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("STMSchedule.Execute(): " + inner.Message, ex);                
            }
        }

        private void SetSkipNumber()
        {
            if (_variantData.Count < _takeNumber)
            {
                // We have been through all products, start over again next time the schedule runs
                _stmSettings.SkipNumber = 0;
            }
            else
            {
                _stmSettings.SkipNumber = _stmSettings.SkipNumber + _takeNumber;
            }
            _settingService.SaveSetting(_stmSettings);
        }

        private void GetData()
        {
            XElement xElementResult = null;
            using (STMService.STM_APISoapClient client = new STMService.STM_APISoapClient(STMService.STM_APISoapClient.EndpointConfiguration.STM_APISoap12, _stmSettings.EndpointAddress))
            {
                // Both timeouts are needed (the call takes about 3 minutes)
                client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 25, 00); // 25 minutes                    
                client.InnerChannel.OperationTimeout = new TimeSpan(0, 25, 00); // 25 minutes

                xElementResult = client.GetItemsAsync().Result;
            }

            if (xElementResult != null && xElementResult.HasElements)
            {
                _variantsNodeList = xElementResult.ToXmlDocument().DocumentElement.ChildNodes;                
            }            
        }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
        private void OrganizeData()
        {
            if(_variantsNodeList == null || _variantsNodeList.Count == 0)
            {
                return;
            }            
            
            VariantData data;
            _variantData = new List<VariantData>();            
            foreach (XmlNode node in _variantsNodeList)
            {
                data = new VariantData();
                data.EAN = node["BARCODE"].InnerText.Trim().Replace("-", "");
                if(string.IsNullOrEmpty(data.EAN) || data.EAN.Length < 10)
                {                    
                    continue;
                }

                data.OrgItemNumber = data.SupplierProductId = node["ITEMNUMBER"].InnerText;
                data.Brand = node["BRAND"].InnerText;
                data.SetSupplierProductId();
                data.OriginalTitle = data.Title = node["ITEMNAME"].InnerText.Trim();
                data.StockCount = Convert.ToInt32(node["INVENTORY"].InnerText.Trim());
                data.RetailPrice = Math.Round(Convert.ToDecimal(node["SALESPRICE"].InnerText.Trim(), CultureInfo.InvariantCulture), 2);                
                data.SizeStr = (node["SIZE"].InnerText.Trim().ToLower() == "stk." || node["SIZE"].InnerText.Trim().ToLower() == "-") ? "" : node["SIZE"].InnerText.Trim();
                data.SetSizeString();
                data.ColorStr = node["COLOR"].InnerText.Trim();

                _variantData.Add(data);
            }

            _variantData = _variantData.Skip(_stmSettings.SkipNumber).Take(_takeNumber).ToList();
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
