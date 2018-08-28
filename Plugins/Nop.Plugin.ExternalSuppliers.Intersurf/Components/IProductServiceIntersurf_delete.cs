using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Components
{
    public interface IProductServiceIntersurf_delete
    {
        ProductAttributeCombination GetProductAttributeCombinationByGtin(string Gtin);
    }
}
