using AO.Services.Extensions;
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

        private IList<Manufacturer> _manufacturers;
        private IList<ProductAttributeCombination> _productAttributeCombinations;
        private IList<Product> _productsSKUAndName; 
        #endregion

        public AOProductService(ILogger logger, IRepository<ProductAttributeCombination> productAttributeCombinationRepository, IProductAttributeService productAttributeService, IManufacturerService manufacturerService, IRepository<Product> productRepository, IRepository<Manufacturer> manufacturerRepository)
        {
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productRepository = productRepository;
            _manufacturerRepository = manufacturerRepository;
            _manufacturerService = manufacturerService;
            _productAttributeService = productAttributeService;
            _logger = logger;

            _productsSKUAndName = GetAllProductsSKUAndName();
        }

        public void SaveVariantData(List<VariantData> variantDataList)
        {
            _manufacturers = _manufacturerService.GetAllManufacturers();
            _productAttributeCombinations = GetAllProductAttributeCombinations();

            foreach (VariantData data in variantDataList.Cleanup())
            {
                try
                {
                    SaveVariant(data);
                }
                catch (Exception ex)
                {
                    Exception inner = ex;
                    while (inner.InnerException != null) inner = inner.InnerException;
                    _logger.Error("SaveVariantData in AOProductService: " + inner.Message, ex);                                        
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

        private void SaveVariant(VariantData data)
        {
            Manufacturer manufacturer = _manufacturers.Where(m => m.Name == data.Brand).FirstOrDefault();
            if (manufacturer == null)
            {
                _logger.Error("STMSchedule, missing manufacturer: '" + data.Brand + "'");
                return;
            }

            ProductAttributeCombination combination = _productAttributeCombinations.Where(p => p.Gtin == data.EAN).FirstOrDefault();
            if (combination == null)
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
                else
                {

                    throw new NotImplementedException("Mangler kode til at tilføje en ny combination");
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
            // Missing API to create product
            //Product product = CreateNewProduct(data.OriginalTitle, data.Title, data.OrgItemNumber, data.Brand, data.RetailPrice, data.CostPrice, data.EAN, data.OriginalCategory, data.ColorStr, data.SizeStr);
            return null;
        } 
        #endregion
    }
}
