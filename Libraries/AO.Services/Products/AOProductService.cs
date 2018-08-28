using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AO.Services.Products
{
    public class AOProductService : IAOProductService
    {
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;

        public AOProductService(IRepository<ProductAttributeCombination> productAttributeCombinationRepository, IRepository<Product> productRepository, IRepository<Manufacturer> manufacturerRepository)
        {
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productRepository = productRepository;
            _manufacturerRepository = manufacturerRepository;
        }


        /// <summary>
        /// Gets a product attribute combination by Gtin
        /// </summary>
        /// <param name="Gtin">Gtin</param>
        /// <returns>Product attribute combination</returns>
        public virtual ProductAttributeCombination GetProductAttributeCombinationByGtin(string Gtin)
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

        public List<Product> GetAllProductsSKUAndName()
        {
            var query = from m in _productRepository.Table                        
                        select new Product() { Sku = m.Sku, Name = m.Name };
            var products = query.ToList();
            return products;
        }

        public Product CreateNewProduct(string originalTitle, string title, string orgItemNumber, string brand, decimal retailPrice, decimal costPrice, string eAN, string originalCategory, string colorStr, string sizeStr)
        {
            return null;
        }

        public List<ProductAttributeCombination> GetAllProductAttributeCombinations()
        {
            var query = from pac in _productAttributeCombinationRepository.Table                      
                        select pac;
            var combinations = query.ToList();
            return combinations;
        }
    }
}
