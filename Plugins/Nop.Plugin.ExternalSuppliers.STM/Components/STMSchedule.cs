
using AO.Services.Logging;
using AO.Services.Products;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.ExternalSuppliers.STM.Extensions;
using Nop.Plugin.ExternalSuppliers.STM.Models;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Messages;
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
        private readonly IAOProductService _aoProductService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private List<VariantData> _variantData;
        private int _totalProductsToHandle, _newlyCreatedProducts = 0, _newlyCreatePSC = 0, _updatedPSCs = 0, _newActivePSCs = 0, _newRemovedPSCs = 0, _leftOutDueToBrand = 0, _leftoutDueToStockType = 0, _leftOutDueToMissingEAN = 0, _leftoutDueToLowStock = 0, _leftOutDueToMissingSizeOrColor = 0;
        private string _leaveOutBrands = ";tentipi;", _extraInfo;
        private IList<Manufacturer> _manufacturers;
        private IList<Product> _productsSKUAndName;
        private IList<ProductAttributeCombination> _productAttributeCombinations;
        private static List<UpdatedProducts> _updatedProducts;
        private static List<UpdatedProducts> _updatedProductsSetActive = new List<UpdatedProducts>();        
        private const string _updaterName = "STM";
        private XmlNodeList _variantsNodeList;

        public STMSchedule(ILogger logger, STMSettings stmSettings, IAOProductService aoProductService, IManufacturerService manufacturerService, IProductAttributeService productAttributeService, IProductService productService, IWorkflowMessageService workflowMessageService)
        {
            this._logger = logger;
            this._stmSettings = stmSettings;
            this._aoProductService = aoProductService;
            this._manufacturerService = manufacturerService;
            this._productAttributeService = productAttributeService;
            this._productService = productService;
            this._workflowMessageService = workflowMessageService;
        }

        public void Execute()
        {
            bool allWell = true;
            try
            {
                ValidateSettings();

                _manufacturers = _manufacturerService.GetAllManufacturers();
                _productsSKUAndName = _aoProductService.GetAllProductsSKUAndName();
                _productAttributeCombinations = _aoProductService.GetAllProductAttributeCombinations();

                GetVariantDataFromWebservice();
                OrganizeDataIntoList();
                SaveVariantData();
            }
            catch (Exception ex)
            {
                allWell = false;
                _logger.Error("STMSchedule.Execute()", ex);
            }

            if (allWell)
            {
                ShowAndLogStatus();
            }
        }

        private void GetVariantDataFromWebservice()
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

        private void OrganizeDataIntoList()
        {
            if(_variantsNodeList == null || _variantsNodeList.Count == 0)
            {
                return;
            }

            _totalProductsToHandle = _variantsNodeList.Count;

            int goodCount = 0, missingBrand = 0, lockedTitle = 0, missingPrice = 0, wrongCount = 0, wrongEAN = 0;
            VariantData data;
            _variantData = new List<VariantData>();            
            foreach (XmlNode node in _variantsNodeList)
            {
                data = new VariantData();
                data.EAN = node["BARCODE"].InnerText.Trim().Replace("-", "");
                if(string.IsNullOrEmpty(data.EAN) || data.EAN.Length < 10)
                {
                    wrongEAN++;
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

                if (string.IsNullOrEmpty(data.Brand))
                {
                    missingBrand++;
                }
                if (data.OriginalTitle.ToLower() == "spærret")
                {
                    lockedTitle++;
                }
                if (data.RetailPrice == 0)
                {
                    missingPrice++;
                }


                if (string.IsNullOrEmpty(data.Brand) || data.OriginalTitle.ToLower() == "spærret" || data.RetailPrice == 0)
                {
                    wrongCount++;
                }
                else
                {
                    goodCount++;                    
                }

                _variantData.Add(data);
            }
      }

        private void SaveVariantData()
        {            
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
            Manufacturer manufacturer = _manufacturers.Where(m => m.Name == data.Brand).FirstOrDefault();
            if(manufacturer == null)
            {
                _logger.Error("STMSchedule, missing manufacturer: '" + data.Brand + "'");
                return;
            }

            ProductAttributeCombination combination = _productAttributeCombinations.Where(p => p.Gtin == data.EAN).FirstOrDefault();
            if(combination == null)
            {
                // Now we have a new EAN number, lets find out if we also need to create a new product.
                Product product = _productsSKUAndName.Where(p => p.Sku == data.OrgItemNumber && p.Name == data.OriginalTitle).FirstOrDefault();
                if (product == null)
                {
                    if (!string.IsNullOrEmpty(data.Brand) && data.OriginalTitle.ToLower() != "spærret" && data.RetailPrice > 0)
                    {
                        // Only create the product if we have valid data
                        product = CreateProduct(data);
                    }
                }                
            }
            else
            {
                combination.StockQuantity = data.StockCount;
                _productAttributeService.UpdateProductAttributeCombination(combination);
            }
            
        }

        private Product CreateProduct(VariantData data)
        {            
            Product product =  _aoProductService.CreateNewProduct(data.OriginalTitle, data.Title, data.OrgItemNumber, data.Brand, data.RetailPrice, data.CostPrice, data.EAN, data.OriginalCategory, data.ColorStr, data.SizeStr);
            return product;
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

        private void ShowAndLogStatus()
        {
            UpdatingStatus status = new UpdatingStatus(_updatedProducts, _updatedProductsSetActive, _updaterName);
            status.CreatedProducts = _newlyCreatedProducts;
            status.CreatedVariants = _newlyCreatePSC;
            status.UpdatedVariants = _updatedPSCs;
            status.NewActiveVariants = _newActivePSCs;
            status.NewRemovedVariants = _newRemovedPSCs;
            status.LeftoutDueToStockType = _leftoutDueToStockType;
            status.LeftOutDueToBrand = _leftOutDueToBrand;
            status.LeftOutDueToLowStock = _leftoutDueToLowStock;
            status.LeftOutDueToMissingEAN = _leftOutDueToMissingEAN;
            status.LeftOutDueToMissingSizeOrColor = _leftOutDueToMissingSizeOrColor;
            status.ExtraInfo = _extraInfo;

            string strStatus = status.BuildStatus();

            _logger.Information(strStatus);
            //send email
            _workflowMessageService.SendContactUsMessage(1, "axelandersen@gmail.com", "NopCommerce Admin", "STM status", strStatus);
        }
    }
}
