﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Value
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
