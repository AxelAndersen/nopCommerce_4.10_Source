using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace AO.Services.Products
{
    public interface IAOProductService
    {
        ProductAttributeCombination GetProductAttributeCombinationByGtin(string gtin);

        //Product GetProductBySKUAndBrandName(string sku, string brandName);

        List<Product> GetAllProductsSKUAndName();

        List<ProductAttributeCombination> GetAllProductAttributeCombinations();

        Product CreateNewProduct(string originalTitle, string title, string orgItemNumber, string brand, decimal retailPrice, decimal costPrice, string eAN, string originalCategory, string colorStr, string sizeStr);
    }
}
