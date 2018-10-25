using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;

namespace Nop.Plugin.Api.Services
{
    public class ProductAttributesApiService : IProductAttributesApiService
    {
        private readonly IRepository<ProductAttribute> _productAttributesRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;

        public ProductAttributesApiService(IRepository<ProductAttribute> productAttributesRepository, IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository)
        {
            _productAttributesRepository = productAttributesRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
        }

        public IList<ProductAttribute> GetProductAttributes(int limit = Configurations.DefaultLimit,
             int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetProductAttributesQuery(sinceId);

            return new ApiList<ProductAttribute>(query, page - 1, limit);
        }

        public int GetProductAttributesCount()
        {
            return GetProductAttributesQuery().Count();
        }

        public IList<SpecificationAttributeOption> GetSpecificationAttributeOptions()
        {
            var query = _specificationAttributeOptionRepository.Table;
            return new ApiList<SpecificationAttributeOption>(query, 0, 10000);
        }

        ProductAttribute IProductAttributesApiService.GetById(int id)
        {
            if (id <= 0)
                return null;

            return _productAttributesRepository.GetById(id);
        }

        private IQueryable<ProductAttribute> GetProductAttributesQuery(int sinceId = Configurations.DefaultSinceId)
        {
            var query = _productAttributesRepository.Table;

            if (sinceId > 0)
            {
                query = query.Where(productAttribute => productAttribute.Id > sinceId);
            }

            query = query.OrderBy(productAttribute => productAttribute.Id);

            return query;
        }
    }
}