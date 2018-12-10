using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Models;
using Nop.Services.Stores;
using static Nop.Plugin.Api.Controllers.ProductsController;

namespace Nop.Plugin.Api.Services
{
    public class ProductApiService : IProductApiService
    {
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryMappingRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        public string UPDATED_KEYWORD { get { return "SEK-Updated-OldPrice"; } }

        public ProductApiService(IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryMappingRepository,
            IRepository<Vendor> vendorRepository,
            IStoreMappingService storeMappingService)
        {
            _productRepository = productRepository;
            _productCategoryMappingRepository = productCategoryMappingRepository;
            _vendorRepository = vendorRepository;
            _storeMappingService = storeMappingService;
        }

        public IList<Product> GetProducts(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
           int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
           int? categoryId = null, string vendorName = null, bool? publishedStatus = null)
        {
            var query = GetProductsQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, vendorName, publishedStatus, ids, categoryId);

            if (sinceId > 0)
            {
                query = query.Where(c => c.Id > sinceId);
            }

            return new ApiList<Product>(query, page - 1, limit);
        }

        public IList<Product> GetAllProducts()
        {
            var query = from p in _productRepository.Table
                        where !p.Deleted
                        select p;
            var products = query.ToList();

            return products;
        }

        public IList<SeoProduct> GetAllSeoProducts()
        {
            var query = from p in _productRepository.Table
                        where !p.Deleted
                        select new SeoProduct() { ProductId = p.Id, ProductName = p.Name };
            var products = query.ToList();
            return products;
        }


        public int GetProductsCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, bool? publishedStatus = null, string vendorName = null,
            int? categoryId = null)
        {
            var query = GetProductsQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, vendorName,
                                         publishedStatus, categoryId: categoryId);

            return query.ToList().Count(p => _storeMappingService.Authorize(p));
        }

        public Product GetProductById(int productId)
        {
            if (productId == 0)
                return null;

            return _productRepository.Table.FirstOrDefault(product => product.Id == productId && !product.Deleted);
        }

        public Product GetProductByIdNoTracking(int productId)
        {
            if (productId == 0)
                return null;

            return _productRepository.Table.FirstOrDefault(product => product.Id == productId && !product.Deleted);
        }

        private IQueryable<Product> GetProductsQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, string vendorName = null,
            bool? publishedStatus = null, IList<int> ids = null, int? categoryId = null)

        {
            var query = _productRepository.Table;

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.Id));
            }

            if (publishedStatus != null)
            {
                query = query.Where(c => c.Published == publishedStatus.Value);
            }

            // always return products that are not deleted!!!
            query = query.Where(c => !c.Deleted);

            if (createdAtMin != null)
            {
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);
            }

            if (createdAtMax != null)
            {
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);
            }

            if (updatedAtMin != null)
            {
                query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);
            }

            if (updatedAtMax != null)
            {
                query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);
            }

            if (!string.IsNullOrEmpty(vendorName))
            {
                query = from vendor in _vendorRepository.Table
                        join product in _productRepository.Table on vendor.Id equals product.VendorId
                        where vendor.Name == vendorName && !vendor.Deleted && vendor.Active
                        select product;
            }

            if (categoryId != null)
            {
                var categoryMappingsForProduct = from productCategoryMapping in _productCategoryMappingRepository.Table
                                                 where productCategoryMapping.CategoryId == categoryId
                                                 select productCategoryMapping;

                query = from product in query
                        join productCategoryMapping in categoryMappingsForProduct on product.Id equals productCategoryMapping.ProductId
                        select product;
            }

            query = query.OrderBy(product => product.Id);

            return query;
        }

        public void UpdatePrice(Product product, decimal exchangeRate)
        {
            // Use this when applying to actual customer price
            //product.Price = ApplyCurrencyMultiplication(product.Price, exchangeRate);
            if (product.OldPrice > 0)
            {
                decimal newOldPrice = ApplyCurrencyMultiplication(product.OldPrice, exchangeRate);
                if (newOldPrice != product.Price)
                {
                    // This means the product is somehow on sale, set old price which will indicate that price is onsale
                    product.OldPrice = newOldPrice;                   
                }
                else
                {
                    // Product is sold at retail price, dont update old price
                }                
            }

            product.MetaKeywords = UPDATED_KEYWORD;
            _productRepository.Update(product);
        }

        private decimal ApplyCurrencyMultiplication(decimal price, decimal exchangeRate)
        {
            price = (price * exchangeRate);

            string tmpPrice = price.ToString("F0");
            tmpPrice = tmpPrice.Substring(0, tmpPrice.Length - 1);
            tmpPrice = tmpPrice + "9";

            price = Convert.ToDecimal(tmpPrice);
            if(price.ToString().EndsWith("09"))
            {
                price -= 10;
            }
            return price;
        }
    }
}