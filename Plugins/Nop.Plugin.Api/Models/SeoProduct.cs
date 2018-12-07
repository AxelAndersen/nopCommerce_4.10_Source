using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Models
{
    [JsonObject(Title = "SeoProduct")]
    public class SeoProduct
    {
        [JsonProperty("id")]
        public int ProductId { get; set; }

        [JsonProperty("productname")]
        public string ProductName { get; set; }

        [JsonProperty("sename")]
        public string SeName { get; set; }

        [JsonProperty("languageid")]
        public int LanguageId { get; set; }

        [JsonProperty("productcategoryids")]
        public List<int> ProductCategoryIds { get; set; }

        [JsonProperty("productcategoriesstring")]
        public string ProductCategoriesString { get; set; }
    }
}
