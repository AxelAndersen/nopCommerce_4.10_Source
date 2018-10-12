using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.SpecificationAttributes
{
    public class SpecificationAttributesOptionRootObjectDto : ISerializableObject
    {
        public SpecificationAttributesOptionRootObjectDto()
        {
            SpecificationAttributeOptions = new List<SpecificationAttributeOptionDto>();
        }

        [JsonProperty("specification_attribute_options")]
        public IList<SpecificationAttributeOptionDto> SpecificationAttributeOptions { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "specification_attribute_option";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(SpecificationAttributeOptionDto);
        }
    }
}
