using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Languages
{
    public class LocalizedNameDto
    {
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        [JsonProperty("language_id")]
        public int? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the localized name
        /// </summary>
        [JsonProperty("localized_name")]
        public string LocalizedName { get; set; }

        [JsonProperty("localized_shortdescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("localized_description")]
        public string Description { get; set; }

        [JsonProperty("localized_metakeywords")]
        public string MetaKeywords { get; set; }

        [JsonProperty("localized_metadescription")]
        public string MetaDescription { get; set; }

        [JsonProperty("localized_metatitle")]
        public string MetaTitle { get; set; }

        [JsonProperty("localized_sename")]
        public string SeName { get; set; }
    }
}
