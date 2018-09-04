using AO.Services.Products.Models;
using System.Collections.Generic;

namespace AO.Services.Products
{
    public interface IAOProductService
    {        
        void SaveVariantData(List<VariantData> variantDataList);
    }
}
