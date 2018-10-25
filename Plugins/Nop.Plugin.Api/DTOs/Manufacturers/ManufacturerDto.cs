using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Api.DTOs.Manufacturers
{    
    [JsonObject(Title = "manufacturer")]
    public class ManufacturerDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }        

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("metadescription")]
        public string MetaDescription { get; set; }

        [JsonProperty("metatitle")]
        public string MetaTitle { get; set; }

        [JsonProperty("pictureid")]
        public int PictureId { get; set; }
    }
}
