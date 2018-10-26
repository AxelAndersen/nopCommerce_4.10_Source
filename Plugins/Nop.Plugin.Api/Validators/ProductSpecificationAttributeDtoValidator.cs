﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.SpecificationAttributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Api.Validators
{
    public class ProductSpecificationAttributeDtoValidator : AbstractValidator<ProductSpecificationAttributeDto>
    {
        public ProductSpecificationAttributeDtoValidator(IHttpContextAccessor httpMethod, Dictionary<string, object> passedPropertyValuePaires)
        {
            if (httpMethod.HttpContext.Request.Method.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                //apply "create" rules
                RuleFor(x => x.Id).Equal(0).WithMessage("id must be zero or null for new records");
                
                ApplyAttributeTypeIdRule();

                if (passedPropertyValuePaires.ContainsKey("specification_attribute_option_id"))
                {
                    ApplySpecificationAttributeOptoinIdRule();
                }
            }
            else if (httpMethod.HttpContext.Request.Method.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                //apply "update" rules
                RuleFor(x => x.Id).GreaterThan(0).WithMessage("invalid id");
                
                if (passedPropertyValuePaires.ContainsKey("attribute_type_id"))
                {
                    ApplyAttributeTypeIdRule();
                }

                if (passedPropertyValuePaires.ContainsKey("specification_attribute_option_id"))
                {
                    ApplySpecificationAttributeOptoinIdRule();
                }
            }
        }


        private void ApplyAttributeTypeIdRule()
        {
            var specificationAttributeTypes = (SpecificationAttributeType[])Enum.GetValues(typeof(SpecificationAttributeType));
            RuleFor(x => x.AttributeTypeId).InclusiveBetween((int)specificationAttributeTypes.First(), (int)specificationAttributeTypes.Last()).WithMessage("invalid attribute type id");
        }

        private void ApplySpecificationAttributeOptoinIdRule()
        {
            RuleFor(x => x.SpecificationAttributeOptionId).GreaterThan(0).WithMessage("invalid specification attribute option id");
        }
    }
}