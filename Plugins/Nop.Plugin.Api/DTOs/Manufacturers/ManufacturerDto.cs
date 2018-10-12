using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Api.DTOs.Manufacturers
{    
    [JsonObject(Title = "manufacturer")]
    public class ManufacturerDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
