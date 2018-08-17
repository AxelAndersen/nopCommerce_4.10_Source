using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.POS.Kaching.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Nop.Plugin.POS.Kaching
{
    public class POSKachingService
    {
        private readonly ILogger _logger;
        private POSKachingSettings _kachingSettings;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISettingService _settingService;
        
        /// Result for test: https://us-central1-ka-ching-base-test.cloudfunctions.net/imports/product?account=-LBPew-z0swWOy1bacZZ&accountkey=55da0abc-e877-4fd0-8612-6c96ac3c78fd&apikey=0690a383-68e0-4716-b27d-726987b6d31f&integration=friliv
        private static string _apiUrl = "https://[Host]/imports/product?account=[Id]&accountkey=[AccountToken]&apikey=[APIToken]&integration=[ImportQueueName]";
        private static string _pingUrl = "https://[Host]/info";

        public POSKachingService(ILogger logger, POSKachingSettings kachingSettings, ISettingService settingService, IPictureService pictureService, IProductAttributeService productAttributeService)
        {
            this._logger = logger;
            this._kachingSettings = kachingSettings;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._productAttributeService = productAttributeService;

            _apiUrl = _apiUrl.Replace("[Host]", this._kachingSettings.POSKaChingHost)
                            .Replace("[Id]", this._kachingSettings.POSKaChingId)
                            .Replace("[AccountToken]", this._kachingSettings.POSKaChingAccountToken)
                            .Replace("[APIToken]", this._kachingSettings.POSKaChingAPIToken)
                            .Replace("[ImportQueueName]", this._kachingSettings.POSKaChingImportQueueName);

            _pingUrl = _pingUrl.Replace("[Host]", this._kachingSettings.POSKaChingHost); 
        }

        public void SaveProduct(string json)
        {
            string result = "";
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Encoding = Encoding.UTF8;

                try
                {
                    result = client.UploadString(_apiUrl, "POST", json);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }            
        }

        public bool TestConnection()
        {
            bool alive = false;           

            using (var client = new WebClient())
            {
                using (client.OpenRead(_pingUrl))
                {
                    alive = true;
                }
            }

            return alive;            
        }

        public string BuildJSONString(Core.Domain.Catalog.Product product)
        {
            KachingProductModel kaChingProduct = new KachingProductModel();

            kaChingProduct.Product = new Models.Product();
            kaChingProduct.Product.Id = product.Id.ToString();
            kaChingProduct.Product.Name = new Description();
            kaChingProduct.Product.Name.Da = kaChingProduct.Product.Name.En = product.Name;
            kaChingProduct.Product.Description = new Description();
            kaChingProduct.Product.Description.Da = kaChingProduct.Product.Description.En = product.FullDescription.StripHTML();
            kaChingProduct.Product.RetailPrice = (long)product.Price;

            foreach (var pp in product.ProductPictures)
            {
                var pictureUrl = _pictureService.GetPictureUrl(pp.PictureId);
                kaChingProduct.Product.ImageUrl = pictureUrl;

                break;
            }

            List<Variant> variants = new List<Variant>();
            List<Dimension> dimensions = GetDimensions(product, ref variants);

            if (variants.Count == 1)
            {
                kaChingProduct.Product.Barcode = variants[0].Barcode;
            }
            else if (variants.Count > 0)
            {
                kaChingProduct.Product.Variants = variants.ToArray();
                kaChingProduct.Product.Dimensions = dimensions.ToArray();
            }

            kaChingProduct.Metadata = new Metadata();
            kaChingProduct.Metadata.Channels = new Channels();
            kaChingProduct.Metadata.Markets = new Markets();
            kaChingProduct.Metadata.Channels.Pos = true;
            kaChingProduct.Metadata.Channels.Online = true;
            kaChingProduct.Metadata.Markets.Dk = true;

            kaChingProduct.Product.Tags = SetTags(product);

            string output = JsonConvert.SerializeObject(kaChingProduct, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
            return output;
        }

        private Tags SetTags(Core.Domain.Catalog.Product product)
        {
            Tags tags = new Tags();

            if (product.ProductCategories != null && product.ProductCategories.Count > 0)
            {
                foreach (ProductCategory cat in product.ProductCategories)
                {
                    switch (cat.Category?.Name)
                    {
                        case "Mens clothing": tags.Herretoj = true; break;
                        case "Womens clothing": tags.Dametoj = true; break;
                        case "Child wear": tags.Bornetoj = true; break;
                        case "Back packs": tags.Rygsaekke = true; break;
                        case "Sleeping bags": tags.Soveposer = true; break;
                        case "Tents": tags.Telte = true; break;
                        case "Cooking": tags.Kogegrej = true; break;
                        case "Travel accessories": tags.Tilbehor = true; break;
                        case "Footwear": tags.Fodtoj = true; break;
                        default: tags.Diverse = true; break;
                    }
                }
            }

            return tags;
        }

        private List<Dimension> GetDimensions(Core.Domain.Catalog.Product product, ref List<Variant> variants)
        {
            List<Dimension> dimensions = new List<Dimension>();
            var combinationValues = _productAttributeService.GetAllProductAttributeCombinations(product.Id);

            int colorAttributeId = 0, sizeAttributeId = 0;
            GetColorAndSizeId(ref colorAttributeId, ref sizeAttributeId);

            Variant variant = null;

            foreach (var combinationValue in combinationValues)
            {
                XmlDocument attributesXml = new XmlDocument();
                attributesXml.LoadXml(combinationValue.AttributesXml);

                Value colorValue = null, sizeValue = null;

                foreach (XmlNode node in attributesXml.DocumentElement)
                {
                    Dimension colorDimension = new Dimension();
                    Dimension sizeDimension = new Dimension();
                    List<Value> colorValues = new List<Value>();
                    List<Value> sizeValues = new List<Value>();

                    var attributeId = Convert.ToInt32(node.Attributes["ID"].Value);
                    var attributeValueId = Convert.ToInt32(node.FirstChild.FirstChild.InnerText);
                    var attributeValue = _productAttributeService.GetProductAttributeValueById(attributeValueId);

                    var mapping = _productAttributeService.GetProductAttributeMappingById(attributeId);
                    string imageUrl = "";
                    if (attributeValue.PictureId > 0)
                    {
                        imageUrl = _pictureService.GetPictureUrl(attributeValue.PictureId);
                    }
                    else if (product.ProductPictures != null && product.ProductPictures.Count > 0)
                    {
                        imageUrl = _pictureService.GetPictureUrl(product.ProductPictures.First().PictureId);
                    }

                    if (mapping.ProductAttributeId == colorAttributeId)
                    {
                        colorDimension.Id = "color";
                        colorDimension.Name = "Color";

                        colorValue = new Value();
                        colorValue.Id = attributeValue.Id.ToString();
                        colorValue.ImageUrl = imageUrl;
                        colorValue.Name = attributeValue.Name;

                        colorValues.Add(colorValue);

                        colorDimension.Values = colorValues.ToArray();
                        dimensions.Add(colorDimension);
                    }
                    else if (mapping.ProductAttributeId == sizeAttributeId)
                    {
                        sizeDimension.Id = "size";
                        sizeDimension.Name = "Size";

                        sizeValue = new Value();
                        sizeValue.Id = attributeValue.Id.ToString();
                        sizeValue.Name = attributeValue.Name;

                        sizeValues.Add(sizeValue);

                        sizeDimension.Values = sizeValues.ToArray();
                        dimensions.Add(sizeDimension);
                    }

                    variant = new Variant();
                    variant.Barcode = combinationValue.Gtin;
                    variant.Id = combinationValue.Id.ToString();
                    variant.ImageUrl = imageUrl;
                    variant.DimensionValues = new DimensionValues();

                    variant.DimensionValues.Color = colorValue != null ? colorValue.Id : "0";
                    variant.DimensionValues.Size = sizeValue != null ? sizeValue.Id : "0";

                    variants.Add(variant);
                }
            }

            return dimensions;
        }

        private void GetColorAndSizeId(ref int colorAttributeId, ref int sizeAttributeId)
        {
            foreach (var att in _productAttributeService.GetAllProductAttributes())
            {
                if (att.Name.ToUpper() == "COLOR")
                {
                    colorAttributeId = att.Id;
                }
                else if (att.Name.ToUpper() == "SIZE")
                {
                    sizeAttributeId = att.Id;
                }
            }
        }

    }
}
