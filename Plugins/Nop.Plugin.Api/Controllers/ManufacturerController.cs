using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Plugin.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Nop.Plugin.Api.DTOs.Languages;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.DTOs.Manufacturers;

namespace Nop.Plugin.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ManufacturerController : BaseApiController
    {
        private readonly IManufacturerApiService _manufacturerApiService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IFactory<Manufacturer> _factory;
        private readonly IDTOHelper _dtoHelper;
        private readonly ILocalizedEntityService _localizedEntityService;

        public ManufacturerController(IManufacturerApiService manufacturerApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IManufacturerService manufacturerService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICustomerService customerService,
            IFactory<Manufacturer> factory,
            IDTOHelper dtoHelper,
            ILocalizedEntityService localizedEntityService) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _manufacturerApiService = manufacturerApiService;
            _manufacturerService = manufacturerService;
            _urlRecordService = urlRecordService;
            _factory = factory;
            _dtoHelper = dtoHelper;
            _localizedEntityService = localizedEntityService;
        }

        /// <summary>
        /// Receive a list of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetManufacturers()
        {
            var allManufacturers = _manufacturerApiService.GetManufacturers();

            IList<ManufacturerDto> manufacturersAsDtos = allManufacturers.Select(manufacturer =>
            {
                return _dtoHelper.PrepareManufacturerDTO(manufacturer);

            }).ToList();

            var manufacturersRootObject = new ManufacturersRootObject()
            {
                Manufacturers = manufacturersAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, null);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/manufacturers")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public IActionResult CreateManufacturer([ModelBinder(typeof(JsonModelBinder<ManufacturerDto>))] Delta<ManufacturerDto> manufacturerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new category
            var manufacturer = _factory.Initialize();
            manufacturerDelta.Merge(manufacturer);
            _manufacturerService.InsertManufacturer(manufacturer);

            // Preparing the result dto of the new category
            var newManufacturerDto = _dtoHelper.PrepareManufacturerDTO(manufacturer);
            var manufacturersRootObject = new ManufacturersRootObject();
            manufacturersRootObject.Manufacturers.Add(newManufacturerDto);

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }
    }
}
