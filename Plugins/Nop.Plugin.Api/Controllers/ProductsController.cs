using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Nop.Plugin.Api.Controllers
{
    using AO.Services.Extensions;
    using AO.SharedServices.Models;
    using DTOs.Errors;
    using JSON.Serializers;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Nop.Core.Domain.Media;
    using Nop.Plugin.Api.DTOs.Languages;
    using Nop.Plugin.Api.Models;
    using Nop.Services.Orders;
    using System.Globalization;

    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : BaseApiController
    {
        private readonly IProductApiService _productApiService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IFactory<Product> _factory;
        private readonly IProductTagService _productTagService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDTOHelper _dtoHelper;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IPictureService _pictureService;

        public ProductsController(IProductApiService productApiService,
                                  IJsonFieldsSerializer jsonFieldsSerializer,
                                  IProductService productService,
                                  IUrlRecordService urlRecordService,
                                  ICustomerActivityService customerActivityService,
                                  ILocalizationService localizationService,
                                  IFactory<Product> factory,
                                  IAclService aclService,
                                  IStoreMappingService storeMappingService,
                                  IStoreService storeService,
                                  ICustomerService customerService,
                                  IDiscountService discountService,
                                  IPictureService pictureService,
                                  IManufacturerService manufacturerService,
                                  IProductTagService productTagService,
                                  IProductAttributeService productAttributeService,
                                  IDTOHelper dtoHelper,
                                  IProductAttributeParser productAttributeParser,
                                  IShoppingCartService shoppingCartService,
                                  ILocalizedEntityService localizedEntityService,
                                  ISpecificationAttributeService specificationAttributeService) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _productApiService = productApiService;
            _factory = factory;
            _manufacturerService = manufacturerService;
            _productTagService = productTagService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _dtoHelper = dtoHelper;
            _productAttributeParser = productAttributeParser;
            _shoppingCartService = shoppingCartService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _specificationAttributeService = specificationAttributeService;
            _pictureService = pictureService;
        }

        /// <summary>
        /// Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProducts(ProductsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var allProducts = _productApiService.GetProducts(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                        parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId, parameters.CategoryId,
                                                                        parameters.VendorName, parameters.PublishedStatus)
                                                .Where(p => StoreMappingService.Authorize(p));

            IList<ProductDto> productsAsDtos = allProducts.Select(product => _dtoHelper.PrepareProductDTO(product)).ToList();

            var productsRootObject = new ProductsRootObjectDto()
            {
                Products = productsAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(productsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [Route("/api/products/all")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetAllProducts()
        {
            var getAllProduct = _productApiService.GetAllProducts().Where(c => StoreMappingService.Authorize(c)).Select(c => new CustomProduct() { Id = c.Id, Name = c.Name, Manufacturer_part_number = c.ManufacturerPartNumber }).ToList();

            var json = JsonConvert.SerializeObject(getAllProduct);

            return new RawJsonActionResult(json);
        }

        public class CustomProduct
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Manufacturer_part_number { get; set; }
        }

        [HttpGet]
        [Route("/api/products/allseoproductswithoutsename")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public List<SeoProduct> GetAllSeoProductsWithOutSeName([FromQuery] int languageId)
        {            
            List<SeoProduct> productsWithoutSeoName = new List<SeoProduct>();
            var allProducts = _productApiService.GetAllProducts();
            foreach (Product product in allProducts)
            {
                string seName = _urlRecordService.GetSeName(product, languageId, false);
                if (string.IsNullOrEmpty(seName))
                {
                    productsWithoutSeoName.Add(new SeoProduct() { LanguageId = languageId, ProductId = product.Id, ProductName = product.Name });
                }
            }
            return productsWithoutSeoName;
        }

        [HttpGet]
        [Route("/api/products/allseoproducts")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public List<SeoProduct> GetAllSeoProducts([FromQuery] int languageId)
        {
            List<SeoProduct> products = new List<SeoProduct>();
            var allProducts = _productApiService.GetAllProducts();
            foreach (Product product in allProducts)
            {
                products.Add(new SeoProduct() { LanguageId = languageId, ProductId = product.Id, ProductName = product.Name });
            }
            return products;
        }
       
        /// <summary>
        /// Receive a count of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/count")]
        [ProducesResponseType(typeof(ProductsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductsCount(ProductsCountParametersModel parameters)
        {
            var allProductsCount = _productApiService.GetProductsCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                       parameters.UpdatedAtMax, parameters.PublishedStatus, parameters.VendorName,
                                                                       parameters.CategoryId);

            var productsCountRootObject = new ProductsCountRootObject()
            {
                Count = allProductsCount
            };

            return Ok(productsCountRootObject);
        }

        /// <summary>
        /// Retrieve product by spcified id
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            var productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/products")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateProduct([ModelBinder(typeof(JsonModelBinder<ProductDto>))] Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            Product product = _factory.Initialize();
            productDelta.Merge(product);

            _productService.InsertProduct(product);

            UpdateProductPictures(product, productDelta.Dto.Images);

            UpdateProductTags(product, productDelta.Dto.Tags);

            UpdateProductManufacturers(product, productDelta.Dto.ManufacturerIds);

            UpdateAssociatedProducts(product, productDelta.Dto.AssociatedProductIds);

            //search engine name
            var seName = _urlRecordService.ValidateSeName(product, productDelta.Dto.SeName, product.Name, true);

            // Maybe languageid should just be 0 here ?
            _urlRecordService.SaveSlug(product, seName, 0);

            UpdateLocales(product, productDelta.Dto.LocalizedNames);

            UpdateProductAttributes(product, productDelta);

            UpdateSpecificProductAttributes(product, productDelta);

            UpdateAclRoles(product, productDelta.Dto.RoleIds);

            UpdateDiscountMappings(product, productDelta.Dto.DiscountIds);

            UpdateStoreMappings(product, productDelta.Dto.StoreIds);

            _productService.UpdateProduct(product);

            CustomerActivityService.InsertActivity("AddNewProduct",
                LocalizationService.GetResource("ActivityLog.AddNewProduct"), product);

            //Attribute Combination

            var warnings = new List<string>();
            if (productDelta.Dto.ProductAttributeCombination != null)
                foreach (var productAttributeCombination in productDelta.Dto.ProductAttributeCombination)
                {

                    var attributesXml = "";
                    //product attributes
                    var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
                        //ignore non-combinable attributes for combinations
                        .Where(x => !x.IsNonCombinable()).ToList();
                    foreach (var attribute in attributes)
                    {
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlType.DropdownList:
                            case AttributeControlType.RadioList:
                            case AttributeControlType.ColorSquares:
                            case AttributeControlType.ImageSquares:
                                {
                                    var ctrlAttributes = "";
                                    if (attribute.ProductAttribute == null)
                                    {
                                        attribute.ProductAttribute = _productAttributeService.GetProductAttributeById(attribute.ProductAttributeId);
                                    }
                                    if (attribute.ProductAttribute.Name.ToLower().Equals("size"))
                                    {
                                        var attributeValues = attribute.ProductAttributeValues.Where(x => x.Name.ToLower().Equals(productAttributeCombination.SizeValueName.ToLower())).FirstOrDefault();
                                        if (attributeValues != null)
                                            ctrlAttributes = attributeValues.Id.ToString();
                                    }
                                    else if (attribute.ProductAttribute.Name.ToLower().Equals("color"))
                                    {
                                        var attributeValues = attribute.ProductAttributeValues.Where(x => x.Name.ToLower().Equals(productAttributeCombination.ColorValueName.ToLower())).FirstOrDefault();
                                        if (attributeValues != null)
                                            ctrlAttributes = attributeValues.Id.ToString();
                                    }

                                    if (!string.IsNullOrEmpty(ctrlAttributes))
                                    {
                                        var selectedAttributeId = int.Parse(ctrlAttributes);
                                        if (selectedAttributeId > 0)
                                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                attribute, selectedAttributeId.ToString());
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    //validate conditional attributes (if specified)
                    foreach (var attribute in attributes)
                    {
                        var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                        if (conditionMet.HasValue && !conditionMet.Value)
                        {
                            attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                        }
                    }

                    //warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                    //    ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));

                    //check whether the same attribute combination already exists
                    var existingCombination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
                    if (existingCombination != null)
                        warnings.Add(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AlreadyExists"));

                    if (!warnings.Any())
                    {
                        //save combination
                        var combination = new ProductAttributeCombination
                        {
                            ProductId = product.Id,
                            AttributesXml = attributesXml,
                            StockQuantity = productAttributeCombination.StockQuantity,
                            AllowOutOfStockOrders = productAttributeCombination.AllowOutOfStockOrders,
                            Sku = productAttributeCombination.Sku,
                            ManufacturerPartNumber = productAttributeCombination.ManufacturerPartNumber,
                            Gtin = productAttributeCombination.Gtin,
                            OverriddenPrice = productAttributeCombination.OverriddenPrice,
                            NotifyAdminForQuantityBelow = productAttributeCombination.NotifyAdminForQuantityBelow,
                        };
                        _productAttributeService.InsertProductAttributeCombination(combination);

                        //quantity change history
                        _productService.AddStockQuantityHistoryEntry(product, combination.StockQuantity, combination.StockQuantity,
                            message: _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                    }

                }

            if (warnings.Any())
            {
                return Error(HttpStatusCode.BadRequest, "productAttributeCombination", string.Join(",", warnings));
            }

            // Preparing the result dto of the new product
            var productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult UpdateProduct([ModelBinder(typeof(JsonModelBinder<ProductDto>))] Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var product = _productApiService.GetProductById(productDelta.Dto.Id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            productDelta.Merge(product);

            product.UpdatedOnUtc = DateTime.UtcNow;
            _productService.UpdateProduct(product);

            UpdateLocales(product, productDelta.Dto.LocalizedNames);

            UpdateSpecificProductAttributes(product, productDelta);

            UpdateProductPictures(product, productDelta.Dto.Images);

            UpdateProductAttributes(product, productDelta);

            UpdateProductTags(product, productDelta.Dto.Tags);

            UpdateProductManufacturers(product, productDelta.Dto.ManufacturerIds);

            UpdateAssociatedProducts(product, productDelta.Dto.AssociatedProductIds);

            // Update the SeName if specified
            if (productDelta.Dto.SeName != null)
            {
                var seName = _urlRecordService.ValidateSeName(product, productDelta.Dto.SeName, product.Name, true);

                // Maybe languageid should just be 0 here ?
                _urlRecordService.SaveSlug(product, seName, 0);
            }

            UpdateDiscountMappings(product, productDelta.Dto.DiscountIds);

            UpdateStoreMappings(product, productDelta.Dto.StoreIds);

            UpdateAclRoles(product, productDelta.Dto.RoleIds);

            _productService.UpdateProduct(product);

            CustomerActivityService.InsertActivity("UpdateProduct",
               LocalizationService.GetResource("ActivityLog.UpdateProduct"), product);

            //Attribute Combination

            var warnings = new List<string>();
            if (productDelta.Dto.ProductAttributeCombination != null)
                foreach (var productAttributeCombination in productDelta.Dto.ProductAttributeCombination)
                {

                    var attributesXml = "";
                    //product attributes
                    var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
                        //ignore non-combinable attributes for combinations
                        .Where(x => !x.IsNonCombinable()).ToList();
                    foreach (var attribute in attributes)
                    {
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlType.DropdownList:
                            case AttributeControlType.RadioList:
                            case AttributeControlType.ColorSquares:
                            case AttributeControlType.ImageSquares:
                                {
                                    var ctrlAttributes = "";
                                    if (attribute.ProductAttribute == null)
                                    {
                                        attribute.ProductAttribute = _productAttributeService.GetProductAttributeById(attribute.ProductAttributeId);
                                    }
                                    if (attribute.ProductAttribute.Name.ToLower().Equals("size"))
                                    {
                                        var attributeValues = attribute.ProductAttributeValues.Where(x => x.Name.ToLower().Equals(productAttributeCombination.SizeValueName.ToLower())).FirstOrDefault();
                                        if (attributeValues != null)
                                            ctrlAttributes = attributeValues.Id.ToString();
                                    }
                                    else if (attribute.ProductAttribute.Name.ToLower().Equals("color"))
                                    {
                                        var attributeValues = attribute.ProductAttributeValues.Where(x => x.Name.ToLower().Equals(productAttributeCombination.ColorValueName.ToLower())).FirstOrDefault();
                                        if (attributeValues != null)
                                            ctrlAttributes = attributeValues.Id.ToString();
                                    }
                                    if (!string.IsNullOrEmpty(ctrlAttributes))
                                    {
                                        var selectedAttributeId = int.Parse(ctrlAttributes);
                                        if (selectedAttributeId > 0)
                                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                attribute, selectedAttributeId.ToString());
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    //validate conditional attributes (if specified)
                    foreach (var attribute in attributes)
                    {
                        var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                        if (conditionMet.HasValue && !conditionMet.Value)
                        {
                            attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                        }
                    }

                    //warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                    //    ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));

                    //check whether the same attribute combination already exists
                    var existingCombination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
                    if (existingCombination != null)
                        warnings.Add(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AlreadyExists"));

                    if (!warnings.Any())
                    {
                        //save combination
                        var combination = new ProductAttributeCombination
                        {
                            ProductId = product.Id,
                            AttributesXml = attributesXml,
                            StockQuantity = productAttributeCombination.StockQuantity,
                            AllowOutOfStockOrders = productAttributeCombination.AllowOutOfStockOrders,
                            Sku = productAttributeCombination.Sku,
                            ManufacturerPartNumber = productAttributeCombination.ManufacturerPartNumber,
                            Gtin = productAttributeCombination.Gtin,
                            OverriddenPrice = productAttributeCombination.OverriddenPrice,
                            NotifyAdminForQuantityBelow = productAttributeCombination.NotifyAdminForQuantityBelow,
                        };
                        _productAttributeService.InsertProductAttributeCombination(combination);

                        //quantity change history
                        _productService.AddStockQuantityHistoryEntry(product, combination.StockQuantity, combination.StockQuantity,
                            message: _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                    }

                }

            if (warnings.Any())
            {
                return Error(HttpStatusCode.BadRequest, "productAttributeCombination", string.Join(",", warnings));
            }


            // Preparing the result dto of the new product
            var productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/productsename/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult UpdateProductSeName([ModelBinder(typeof(JsonModelBinder<ProductDto>))] Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var product = _productApiService.GetProductById(productDelta.Dto.Id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            // Update the SeName if specified
            if (productDelta.Dto.SeName != null)
            {
                var seName = _urlRecordService.ValidateSeName(product, productDelta.Dto.SeName.ReplaceSpecialCharacters(), product.Name, true);

                // Maybe languageid should just be 0 here ?
                _urlRecordService.SaveSlug(product, seName, 0);
            }

            // Preparing the result dto of the new product
            var productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/updatelanguagespecificproductsename")]
        public void UpdateLanguageSpecificProductSeName([ModelBinder(typeof(JsonModelBinder<SeoProduct>))] Delta<SeoProduct> seoProduct)
        {
            var product = _productApiService.GetProductById(seoProduct.Dto.ProductId);

            if (product == null)
            {
                throw new ArgumentException("Wrong id, no product found with id: " + seoProduct.Dto.ProductId);
            }

            if (seoProduct.Dto.LanguageId <= 0)
            {
                throw new ArgumentException("You must specify the language id");
            }

            if (string.IsNullOrEmpty(seoProduct.Dto.SeName))
            {
                throw new ArgumentException("You must specify the seName to use with langaugeid: " + seoProduct.Dto.LanguageId);
            }

            _urlRecordService.SaveSlug(product, seoProduct.Dto.SeName, seoProduct.Dto.LanguageId);
        }

        [HttpPut]
        [Route("/api/updatepriceswithexchangerate")]
        public IActionResult UpdatePricesWithExchangeRate([FromQuery]string exchangeRate)
        {
            IResult result = new Result();

            decimal newExchangeRate = decimal.Parse(exchangeRate.Replace(",", "."), CultureInfo.InvariantCulture);
            var products = _productApiService.GetAllProducts();

            if (products == null || products.Count == 0)
            {
                result.ErrorMessage = "No products found in UpdatePricesForCurrency()";
                result.Success = false;
                return result as IActionResult;
            }

            if (newExchangeRate == 0)
            {
                result.ErrorMessage = "ExchangeRate cant be 0";
                result.Success = false;
                return result as IActionResult;                
            }

            result.Success = true;
            foreach (Product product in products)
            {
                try
                {
                    if(product.MetaKeywords == "SEK-Updated")
                    {
                        continue;
                    }

                    _productApiService.UpdatePrice(product, newExchangeRate);
                    result.EntityCount++;
                }
                catch(Exception ex)
                {
                    result.ErrorMessage += Environment.NewLine + product.Name + ": " + ex.Message;
                    result.Success = false;
                }
            }
           
            return result as IActionResult;
        }

        [HttpDelete]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            _productService.DeleteProduct(product);

            //activity log
            CustomerActivityService.InsertActivity("DeleteProduct",
                string.Format(LocalizationService.GetResource("ActivityLog.DeleteProduct"), product.Name), product);

            return new RawJsonActionResult("{}");
        }

        private void UpdateProductPictures(Product entityToUpdate, List<ImageMappingDto> setPictures)
        {
            // If no pictures are specified means we don't have to update anything
            if (setPictures == null)
                return;

            // delete unused product pictures
            var unusedProductPictures = entityToUpdate.ProductPictures.Where(x => setPictures.All(y => y.Id != x.Id)).ToList();
            foreach (var unusedProductPicture in unusedProductPictures)
            {
                var picture = PictureService.GetPictureById(unusedProductPicture.PictureId);
                if (picture == null)
                    throw new ArgumentException("No picture found with the specified id");
                PictureService.DeletePicture(picture);
            }

            foreach (var imageDto in setPictures)
            {
                if (imageDto.Id > 0)
                {
                    // update existing product picture
                    var productPictureToUpdate = entityToUpdate.ProductPictures.FirstOrDefault(x => x.Id == imageDto.Id);
                    if (productPictureToUpdate != null && imageDto.Position > 0)
                    {
                        productPictureToUpdate.DisplayOrder = imageDto.Position;
                        _productService.UpdateProductPicture(productPictureToUpdate);
                    }
                }
                else
                {
                    // add new product picture
                    var newPicture = PictureService.InsertPicture(imageDto.Binary, imageDto.MimeType, string.Empty);
                    _productService.InsertProductPicture(new ProductPicture()
                    {
                        PictureId = newPicture.Id,
                        ProductId = entityToUpdate.Id,
                        DisplayOrder = imageDto.Position
                    });
                }
            }
        }

        private void UpdateProductAttributes(Product entityToUpdate, Delta<ProductDto> productDtoDelta)
        {
            // If no product attribute mappings are specified means we don't have to update anything
            if (productDtoDelta.Dto.ProductAttributeMappings == null)
                return;

            // delete unused product attribute mappings
            var toBeUpdatedIds = productDtoDelta.Dto.ProductAttributeMappings.Where(y => y.Id != 0).Select(x => x.Id);

            var unusedProductAttributeMappings = entityToUpdate.ProductAttributeMappings.Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (var unusedProductAttributeMapping in unusedProductAttributeMappings)
            {
                _productAttributeService.DeleteProductAttributeMapping(unusedProductAttributeMapping);
            }

            foreach (var productAttributeMappingDto in productDtoDelta.Dto.ProductAttributeMappings)
            {
                if (productAttributeMappingDto.Id > 0)
                {
                    // update existing product attribute mapping
                    var productAttributeMappingToUpdate = entityToUpdate.ProductAttributeMappings.FirstOrDefault(x => x.Id == productAttributeMappingDto.Id);
                    if (productAttributeMappingToUpdate != null)
                    {
                        productDtoDelta.Merge(productAttributeMappingDto, productAttributeMappingToUpdate, false);

                        _productAttributeService.UpdateProductAttributeMapping(productAttributeMappingToUpdate);

                        UpdateProductAttributeValues(productAttributeMappingDto, productDtoDelta, entityToUpdate);
                    }
                }
                else
                {
                    var newProductAttributeMapping = new ProductAttributeMapping { ProductId = entityToUpdate.Id };

                    productDtoDelta.Merge(productAttributeMappingDto, newProductAttributeMapping);

                    // add new product attribute
                    _productAttributeService.InsertProductAttributeMapping(newProductAttributeMapping);
                    foreach (var value in productAttributeMappingDto.ProductAttributeValues)
                    {
                        if (value.ImageSquaresImage == null)
                            continue;

                        var image = value.ImageSquaresImage;
                        var imageValidationAttribute = new ImageValidationAttribute();
                        imageValidationAttribute.Validate(image);

                        Picture newPicture = _pictureService.InsertPicture(image.Binary, image.MimeType, string.Empty);
                        if (newProductAttributeMapping.ProductAttributeValues != null)
                        {
                            newProductAttributeMapping.ProductAttributeValues.FirstOrDefault(x => x.PictureId == 0)
                                .ImageSquaresPictureId = newPicture.Id;

                            if (value.ProductPictureId.HasValue)
                            {
                                if (productDtoDelta.Dto.Images.Select(x => x.Id).Contains(value.ProductPictureId.Value) == false)
                                {
                                    var productpicture = _productService.GetProductPicturesByProductId(entityToUpdate.Id);
                                    if (productpicture != null)
                                    {
                                        var getImage = productpicture.Where(x => x.DisplayOrder == value.ProductPictureId.Value).FirstOrDefault();
                                        if (getImage != null)
                                            newProductAttributeMapping.ProductAttributeValues.FirstOrDefault(x => x.PictureId == 0).PictureId = getImage.PictureId;
                                    }
                                }
                                else
                                {
                                    var productpicture = _productService.GetProductPicturesByProductId(entityToUpdate.Id);
                                    if (productpicture != null)
                                    {
                                        var getImage = productpicture.FirstOrDefault(x => x.Id == value.ProductPictureId);
                                        if (getImage != null)
                                        {
                                            newProductAttributeMapping.ProductAttributeValues.FirstOrDefault(x => x.PictureId == 0).PictureId = getImage.PictureId;
                                        }

                                    }
                                }
                            }
                        }
                    }
                    // add new product attribute
                    _productAttributeService.UpdateProductAttributeMapping(newProductAttributeMapping);
                }
            }
        }

        private void UpdateProductAttributeValues(ProductAttributeMappingDto productAttributeMappingDto, Delta<ProductDto> productDtoDelta, Product product)
        {
            // If no product attribute values are specified means we don't have to update anything
            if (productAttributeMappingDto.ProductAttributeValues == null)
                return;

            // delete unused product attribute values
            var toBeUpdatedIds = productAttributeMappingDto.ProductAttributeValues.Where(y => y.Id != 0).Select(x => x.Id);

            var unusedProductAttributeValues =
                _productAttributeService.GetProductAttributeValues(productAttributeMappingDto.Id).Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (var unusedProductAttributeValue in unusedProductAttributeValues)
            {
                _productAttributeService.DeleteProductAttributeValue(unusedProductAttributeValue);
            }

            foreach (var productAttributeValueDto in productAttributeMappingDto.ProductAttributeValues)
            {
                if (productAttributeValueDto.Id > 0)
                {
                    // update existing product attribute mapping
                    var productAttributeValueToUpdate =
                        _productAttributeService.GetProductAttributeValueById(productAttributeValueDto.Id);
                    if (productAttributeValueToUpdate != null)
                    {
                        if (productDtoDelta.Dto.Images != null)
                        {
                            foreach (var item in productDtoDelta.Dto.ProductAttributeMappings)
                            {
                                foreach (var i in item.ProductAttributeValues)
                                {
                                    if (i.ImageSquaresImage == null)
                                        continue;

                                    if (i.ImageSquaresImage.Id == 0)
                                    {
                                        var getOldPicture = _pictureService.GetPictureById(productAttributeValueToUpdate.ImageSquaresPictureId);
                                        if (getOldPicture != null)
                                            _pictureService.DeletePicture(getOldPicture);

                                        if (i.ImageSquaresImage.Src == null)
                                            continue;

                                        var image = i.ImageSquaresImage;
                                        var imageValidationAttribute = new ImageValidationAttribute();
                                        imageValidationAttribute.Validate(image);

                                        Picture newPicture = _pictureService.InsertPicture(image.Binary, image.MimeType, string.Empty);
                                        if (productAttributeValueToUpdate != null)
                                        {
                                            productAttributeValueToUpdate.ImageSquaresPictureId = newPicture.Id;
                                        }
                                    }
                                    if (i.ProductPictureId.HasValue)
                                    {
                                        if (productDtoDelta.Dto.Images.Select(x => x.Id).Contains(i.ProductPictureId.Value) == false)
                                        {
                                            var productpicture = _productService.GetProductPicturesByProductId(product.Id);
                                            if (productpicture != null)
                                            {
                                                var getImage = productpicture.Where(x => x.DisplayOrder == i.ProductPictureId.Value).FirstOrDefault();
                                                if (getImage != null)
                                                    productAttributeValueToUpdate.PictureId = getImage.PictureId;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        productDtoDelta.Merge(productAttributeValueDto, productAttributeValueToUpdate, false);

                        _productAttributeService.UpdateProductAttributeValue(productAttributeValueToUpdate);
                    }
                }
                else
                {
                    var newProductAttributeValue = new ProductAttributeValue();
                    productDtoDelta.Merge(productAttributeValueDto, newProductAttributeValue);

                    newProductAttributeValue.ProductAttributeMappingId = productAttributeMappingDto.Id;
                    // add new product attribute value
                    _productAttributeService.InsertProductAttributeValue(newProductAttributeValue);
                }
            }
        }

        private void UpdateProductTags(Product product, IReadOnlyCollection<string> productTags)
        {
            if (productTags == null)
                return;

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //Copied from UpdateProductTags method of ProductTagService
            //product tags
            var existingProductTags = _productTagService.GetAllProductTagsByProductId(product.Id);
            var productTagsToRemove = new List<ProductTag>();
            foreach (var existingProductTag in existingProductTags)
            {
                var found = false;
                foreach (var newProductTag in productTags)
                {
                    if (!existingProductTag.Name.Equals(newProductTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                {
                    productTagsToRemove.Add(existingProductTag);
                }
            }

            foreach (var productTag in productTagsToRemove)
            {
                //product.ProductTags.Remove(productTag);
                product.ProductProductTagMappings
                    .Remove(product.ProductProductTagMappings.FirstOrDefault(mapping => mapping.ProductTagId == productTag.Id));
                _productService.UpdateProduct(product);
            }

            foreach (var productTagName in productTags)
            {
                ProductTag productTag;
                var productTag2 = _productTagService.GetProductTagByName(productTagName);
                if (productTag2 == null)
                {
                    //add new product tag
                    productTag = new ProductTag
                    {
                        Name = productTagName
                    };
                    _productTagService.InsertProductTag(productTag);
                }
                else
                {
                    productTag = productTag2;
                }

                if (!_productService.ProductTagExists(product, productTag.Id))
                {
                    product.ProductProductTagMappings.Add(new ProductProductTagMapping { ProductTag = productTag });
                    _productService.UpdateProduct(product);
                }

                var seName = _urlRecordService.ValidateSeName(productTag, string.Empty, productTag.Name, true);
                _urlRecordService.SaveSlug(productTag, seName, 0);
            }
        }

        private void UpdateDiscountMappings(Product product, List<int> passedDiscountIds)
        {
            if (passedDiscountIds == null)
                return;

            var allDiscounts = DiscountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);

            foreach (var discount in allDiscounts)
            {
                if (passedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                        product.AppliedDiscounts.Add(discount);
                }
                else
                {
                    //remove discount
                    if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                        product.AppliedDiscounts.Remove(discount);
                }
            }

            _productService.UpdateProduct(product);
            _productService.UpdateHasDiscountsApplied(product);
        }

        private void UpdateProductManufacturers(Product product, List<int> passedManufacturerIds)
        {
            // If no manufacturers specified then there is nothing to map 
            if (passedManufacturerIds == null)
                return;

            var unusedProductManufacturers = product.ProductManufacturers.Where(x => !passedManufacturerIds.Contains(x.ManufacturerId)).ToList();

            // remove all manufacturers that are not passed
            foreach (var unusedProductManufacturer in unusedProductManufacturers)
            {
                _manufacturerService.DeleteProductManufacturer(unusedProductManufacturer);
            }

            foreach (var passedManufacturerId in passedManufacturerIds)
            {
                // not part of existing manufacturers so we will create a new one
                if (product.ProductManufacturers.All(x => x.ManufacturerId != passedManufacturerId))
                {
                    // if manufacturer does not exist we simply ignore it, otherwise add it to the product
                    var manufacturer = _manufacturerService.GetManufacturerById(passedManufacturerId);
                    if (manufacturer != null)
                    {
                        _manufacturerService.InsertProductManufacturer(new ProductManufacturer()
                        { ProductId = product.Id, ManufacturerId = manufacturer.Id });
                    }
                }
            }
        }

        private void UpdateAssociatedProducts(Product product, List<int> passedAssociatedProductIds)
        {
            // If no associated products specified then there is nothing to map 
            if (passedAssociatedProductIds == null)
                return;

            var noLongerAssociatedProducts =
                _productService.GetAssociatedProducts(product.Id, showHidden: true)
                    .Where(p => !passedAssociatedProductIds.Contains(p.Id));

            // update all products that are no longer associated with our product
            foreach (var noLongerAssocuatedProduct in noLongerAssociatedProducts)
            {
                noLongerAssocuatedProduct.ParentGroupedProductId = 0;
                _productService.UpdateProduct(noLongerAssocuatedProduct);
            }

            var newAssociatedProducts = _productService.GetProductsByIds(passedAssociatedProductIds.ToArray());
            foreach (var newAssociatedProduct in newAssociatedProducts)
            {
                newAssociatedProduct.ParentGroupedProductId = product.Id;
                _productService.UpdateProduct(newAssociatedProduct);
            }
        }

        protected virtual void UpdateLocales(Product product, List<LocalizedNameDto> localizedNameDto)
        {
            if (localizedNameDto == null)
                return;

            foreach (var localized in localizedNameDto)
            {
                _localizedEntityService.SaveLocalizedValue(product,
                      x => x.Name,
                      localized.LocalizedName,
                      localized.LanguageId.Value);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.ShortDescription,
                    localized.ShortDescription,
                    localized.LanguageId.Value);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.FullDescription,
                    localized.Description,
                    localized.LanguageId.Value);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId.Value);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId.Value);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId.Value);

                var seName = _urlRecordService.ValidateSeName(product, localized.LocalizedName, product.Name, false);
                _urlRecordService.SaveSlug(product, seName, localized.LanguageId.Value);

            }
        }

        private void UpdateSpecificProductAttributes(Product entityToUpdate, Delta<ProductDto> productDtoDelta)
        {
            // If no product specific attribute mappings are specified means we don't have to update anything
            if (productDtoDelta.Dto.ProductSpecificationAttributes == null)
                return;

            // delete unused product specific attribute mappings
            IEnumerable<int> toBeUpdatedIds = productDtoDelta.Dto.ProductSpecificationAttributes.Where(y => y.Id != 0).Select(x => x.Id);

            var unusedProductSpecificationAttributeMappings = entityToUpdate.ProductSpecificationAttributes.Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (var upsam in unusedProductSpecificationAttributeMappings)
            {
                _specificationAttributeService.DeleteProductSpecificationAttribute(upsam);
            }

            foreach (var ProductSpecificationAttributeDto in productDtoDelta.Dto.ProductSpecificationAttributes)
            {
                if (ProductSpecificationAttributeDto.Id > 0)
                {
                    // update existing product attribute mapping
                    var productSpecificationAttributeMappingToUpdate = entityToUpdate.ProductSpecificationAttributes.FirstOrDefault(x => x.Id == ProductSpecificationAttributeDto.Id);
                    if (productSpecificationAttributeMappingToUpdate != null)
                    {
                        productDtoDelta.Merge(ProductSpecificationAttributeDto, productSpecificationAttributeMappingToUpdate, false);

                        _specificationAttributeService.UpdateProductSpecificationAttribute(productSpecificationAttributeMappingToUpdate);

                        //TODO: [Div] - update product specific option if required
                        //UpdateProductSpecificationAttributeValues(productAttributeMappingDto, productDtoDelta);
                    }
                }
                else
                {
                    ProductSpecificationAttribute newProductSpecificationAttributeMapping = new ProductSpecificationAttribute();
                    newProductSpecificationAttributeMapping.ProductId = entityToUpdate.Id;

                    productDtoDelta.Merge(ProductSpecificationAttributeDto, newProductSpecificationAttributeMapping);

                    // add new product specification attribute
                    _specificationAttributeService.InsertProductSpecificationAttribute(newProductSpecificationAttributeMapping);
                }
            }
        }
    }
}