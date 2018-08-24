using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Components
{
    public class ProductService : IProductService
    {
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;

        public ProductService(IRepository<ProductAttributeCombination> productAttributeCombinationRepository)
        {
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
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
    }
}
