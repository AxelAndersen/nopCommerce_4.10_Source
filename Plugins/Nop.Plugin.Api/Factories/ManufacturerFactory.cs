using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.Api.Factories
{
    public class ManufacturerFactory : IFactory<Manufacturer>
    {
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        
        public ManufacturerFactory(IManufacturerTemplateService manufacturerTemplateService)
        {
            _manufacturerTemplateService = manufacturerTemplateService;            
        }

        public Manufacturer Initialize()
        {
            // TODO: cache the default entity.
            var defaultManufacturer = new Manufacturer();

            // Set the first template as the default one.
            var firstTemplate = _manufacturerTemplateService.GetAllManufacturerTemplates().FirstOrDefault();

            if (firstTemplate != null)
            {
                defaultManufacturer.ManufacturerTemplateId = firstTemplate.Id;
            }

            //default values
            defaultManufacturer.PageSize = 9;
            defaultManufacturer.PageSizeOptions = "9, 25, 50";
            defaultManufacturer.Published = true;            
            defaultManufacturer.AllowCustomersToSelectPageSize = true;

            defaultManufacturer.CreatedOnUtc = DateTime.UtcNow;
            defaultManufacturer.UpdatedOnUtc = DateTime.UtcNow;

            return defaultManufacturer;
        }
    }
}
