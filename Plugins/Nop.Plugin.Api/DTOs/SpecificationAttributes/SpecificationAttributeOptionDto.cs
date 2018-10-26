﻿using FluentValidation.Attributes;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Languages;
using Nop.Plugin.Api.Validators;
using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.SpecificationAttributes
{
    [JsonObject(Title = "specification_attribute_option")]
    [Validator(typeof(SpecificationAttributeOptionDtoValidator))]
    public class SpecificationAttributeOptionDto
    {
        private List<LocalizedNameDto> _localizedNames;

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        [JsonProperty("specification_attribute_id")]
        public int SpecificationAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color RGB value (used when you want to display "Color squares" instead of text)
        /// </summary>
        [JsonProperty("color_squares_rgb")]
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int DisplayOrder { get; set; }

        [JsonProperty("localized_names")]
        public List<LocalizedNameDto> LocalizedNames
        {
            get
            {
                return _localizedNames;
            }
            set
            {
                _localizedNames = value;
            }
        }
    }
}