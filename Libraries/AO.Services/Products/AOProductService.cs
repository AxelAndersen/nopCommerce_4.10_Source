using AO.Services.Dictionaries;
using AO.Services.Emails;
using AO.Services.Extensions;
using AO.Services.Logging;
using AO.Services.Products.Models;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AO.Services.Products
{
    public class AOProductService : IAOProductService
    {
        #region Private variables
        private readonly ILogger _logger;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IMessageService _messageService;

        private IList<Manufacturer> _manufacturers;
        private IList<ProductAttributeCombination> _productAttributeCombinations;
        private IList<Product> _productsSKUAndName;
        private List<UpdatedProducts> _updatedProducts;
        private List<UpdatedProducts> _updatedProductsSetActive;
        private string _updaterName;
        private int _newlyCreatedProducts, _newlyCreatePSC, _updatedPSCs;
        private List<VariantData> _variantDataToBeCreated;
        #endregion

        public AOProductService(ILogger logger, IMessageService messageService, IRepository<ProductAttributeCombination> productAttributeCombinationRepository, IProductAttributeService productAttributeService, IManufacturerService manufacturerService, IRepository<Product> productRepository, IRepository<Manufacturer> manufacturerRepository)
        {
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productRepository = productRepository;
            _manufacturerRepository = manufacturerRepository;
            _manufacturerService = manufacturerService;
            _productAttributeService = productAttributeService;
            _logger = logger;
            _messageService = messageService;

            _productsSKUAndName = GetAllProductsSKUAndName();
            _variantDataToBeCreated = new List<VariantData>();
        }

        public void SaveVariantData(List<VariantData> variantDataList, string updaterName)
        {
            _manufacturers = _manufacturerService.GetAllManufacturers();
            _productAttributeCombinations = GetAllProductAttributeCombinations();
            _updaterName = updaterName;
            _updatedProducts = new List<UpdatedProducts>();
            _updatedProductsSetActive = new List<UpdatedProducts>();

            foreach (VariantData data in variantDataList.Cleanup())
            {
                try
                {
                    UpdateStock(data);
                }
                catch (Exception ex)
                {
                    Exception inner = ex;
                    while (inner.InnerException != null) inner = inner.InnerException;
                    _logger.Error("UpdateStock() in SaveVariantData in AOProductService: " + inner.Message, ex);
                }
            }

            foreach (VariantData data in _variantDataToBeCreated.CleanupForCreation())
            {
                try
                {
                    SaveNewVariant(data);
                }
                catch (Exception ex)
                {
                    Exception inner = ex;
                    while (inner.InnerException != null) inner = inner.InnerException;
                    _logger.Error("UpdateStock() in SaveVariantData in AOProductService: " + inner.Message, ex);
                }
            }
        }

        #region Private methods
        /// <summary>
        /// Gets a product attribute combination by Gtin
        /// </summary>
        /// <param name="Gtin">Gtin</param>
        /// <returns>Product attribute combination</returns>
        private ProductAttributeCombination GetProductAttributeCombinationByGtin(string Gtin)
        {
            if (String.IsNullOrEmpty(Gtin))
                return null;

            Gtin = Gtin.Trim();

            var query = from pac in _productAttributeCombinationRepository.Table
                        orderby pac.Gtin
                        where pac.Gtin == Gtin
                        select pac;
            var combination = query.FirstOrDefault();
            return combination;
        }

        private List<Product> GetAllProductsSKUAndName()
        {
            var query = from m in _productRepository.Table
                        select new Product() { Sku = m.Sku, Name = m.Name };
            var products = query.ToList();
            return products;
        }

        private List<ProductAttributeCombination> GetAllProductAttributeCombinations()
        {
            var query = from pac in _productAttributeCombinationRepository.Table
                        select pac;
            var combinations = query.ToList();
            return combinations;
        }

        private void UpdateStock(VariantData data)
        {
            ProductAttributeCombination combination = _productAttributeCombinations.Where(p => p.Gtin == data.EAN).FirstOrDefault();
            if(combination == null)
            {
                _variantDataToBeCreated.Add(data);
            }
            else
            {
                combination.StockQuantity = data.StockCount;
                _productAttributeService.UpdateProductAttributeCombination(combination);
                _updatedPSCs++;
            }
        }

        private void SaveNewVariant(VariantData data)
        {
            Manufacturer manufacturer = _manufacturers.Where(m => m.Name == data.Brand).FirstOrDefault();
            if (manufacturer == null)
            {
                var manuturerName = GetAssociatedManufacturerName(data.Brand);
                manufacturer = _manufacturers.Where(m => m.Name == data.Brand).FirstOrDefault();
                if (manufacturer == null)
                {
                    _logger.Error("SaveVariant, missing manufacturer: '" + data.Brand + "'");
                    return;
                }
            }        

            ProductAttributeCombination combination = _productAttributeCombinations.Where(p => p.Gtin == data.EAN).FirstOrDefault();
            if (combination == null)
            {
                // Now we have a new EAN number, lets find out if we also need to create a new product.
                Product product = _productsSKUAndName.Where(p => p.Sku == data.OrgItemNumber && p.Name == data.OriginalTitle).FirstOrDefault();
                if (product == null)
                {
                    // Only create the product if we have valid data
                    product = CreateProduct(data);
                    _newlyCreatedProducts++;
                }
                else
                {
                    _newlyCreatePSC++;
                    throw new NotImplementedException("Mangler kode til at tilføje en ny combination");
                }
            }           
        }

        private string GetAssociatedManufacturerName(string brand)
        {
            string associatedBrandName = brand;

            if (BrandAssociations.Brands.ContainsKey(brand))
            {
                associatedBrandName = BrandAssociations.Brands[brand];
            }
            return associatedBrandName;
        }

        private Product CreateProduct(VariantData data)
        {
            // Missing API to create product
            //Product product = CreateNewProduct(data.OriginalTitle, data.Title, data.OrgItemNumber, data.Brand, data.RetailPrice, data.CostPrice, data.EAN, data.OriginalCategory, data.ColorStr, data.SizeStr);
            return null;
        }

        private void ShowAndLogStatus()
        {
            UpdatingStatus status = new UpdatingStatus(_updatedProducts, _updatedProductsSetActive, _updaterName);
            status.CreatedProducts = _newlyCreatedProducts;
            status.CreatedVariants = _newlyCreatePSC;
            status.UpdatedVariants = _updatedPSCs;                       

            string strStatus = status.BuildStatus();

            _logger.Information(strStatus);
            //send email
            _messageService.SendAdminEmail("axelandersen@gmail.com", "NopCommerce Admin", "STM status: " + strStatus);
        }
        #endregion
    }
}
