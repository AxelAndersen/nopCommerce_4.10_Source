using Newtonsoft.Json;

namespace Nop.Plugin.Api.Models
{
    [JsonObject(Title = "SeoCategory")]
    public class SeoCategory
    {
        [JsonProperty("id")]
        public int CategoryId { get; set; }

        [JsonProperty("categoryname")]
        public string CategoryName { get; set; }

        [JsonProperty("sename")]
        public string SeName { get; set; }

        [JsonProperty("languageid")]
        public int LanguageId { get; set; }
    }
}
