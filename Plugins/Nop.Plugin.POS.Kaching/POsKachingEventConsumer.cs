using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Events;
using Nop.Plugin.POS.Kaching.Models;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Nop.Plugin.POS.Kaching
{
    public class POSKachingEventConsumer : IConsumer<EntityUpdatedEvent<Core.Domain.Catalog.Product>> //, IConsumer<EntityInsertedEvent<Product>>
    {
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;

        private readonly MediaSettings _mediaSettings;


        public POSKachingEventConsumer(ILogger logger, IProductService productService, IPictureService pictureService, IProductAttributeService productAttributeService)
        {
            this._logger = logger;
            this._productService = productService;
            this._pictureService = pictureService;
            this._productAttributeService = productAttributeService;
        }

        public void HandleEvent(EntityUpdatedEvent<Core.Domain.Catalog.Product> eventMessage)
        {
            _logger.Information("Handling product update in POS Kaching Plugin");
            _logger.Information("Product Id: " + eventMessage.Entity.Id);

            var product = _productService.GetProductById(eventMessage.Entity.Id);
            if (product == null || product.Deleted)
            {
                _logger.Error("No product found with id: " + eventMessage.Entity.Id);
            }
            else
            {
                var message = $"Product {product.Name} Found";
                _logger.Warning(message);
            }

            var prod = BuildJSONString(product);
        }

        private string BuildJSONString(Core.Domain.Catalog.Product product)
        {
            KachingProductModel kaChingProduct = new KachingProductModel();

            kaChingProduct.Product = new Models.Product();
            kaChingProduct.Product.Id = product.Id.ToString();
            kaChingProduct.Product.Name = new Description();
            kaChingProduct.Product.Name.Da = kaChingProduct.Product.Name.En = product.Name;
            kaChingProduct.Product.Description = new Description();
            kaChingProduct.Product.Description.Da = kaChingProduct.Product.Description.En = product.FullDescription;
            kaChingProduct.Product.RetailPrice = (long)product.Price;

            foreach (var pp in product.ProductPictures)
            {
                var pictureUrl = _pictureService.GetPictureUrl(pp.PictureId);
                kaChingProduct.Product.ImageUrl = pictureUrl;

                break;
            }

            _logger.Warning("kaChingProduct.Product.ImageUrl: " + kaChingProduct.Product.ImageUrl);

            List<Variant> variants = new List<Variant>();
            List<Dimension> dimensions = GetDimensions(product, ref variants);

            //if (variants.Count == 1)
            //{
            //    kaChingProduct.Product.Barcode = variants[0].Barcode;
            //}
            //else if (variants.Count > 0)
            //{
            //    kaChingProduct.Product.Variants = variants.ToArray();
            //    kaChingProduct.Product.Dimensions = dimensions.ToArray();
            //}

            //kaChingProduct.Metadata = new Metadata();
            //kaChingProduct.Metadata.Channels = new Channels();
            //kaChingProduct.Metadata.Markets = new Markets();
            //kaChingProduct.Metadata.Channels.Pos = true;
            //kaChingProduct.Metadata.Channels.Online = true;
            //kaChingProduct.Metadata.Markets.Dk = true;

            //kaChingProduct.Product.Tags = SetTags(product);

            //string output = JsonConvert.SerializeObject(kaChingProduct, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
            //return output;

            return "";
        }

        private List<Dimension> GetDimensions(Core.Domain.Catalog.Product product, ref List<Variant> variants)
        {
            List<Dimension> dimensions = null;


            var combinationValues = _productAttributeService.GetAllProductAttributeCombinations(product.Id);

            int colorAttributeId = 0, sizeAttributeId = 0;
            GetColorAndSizeId(ref colorAttributeId, ref sizeAttributeId);

            Variant variant = null;

            foreach (var combinationValue in combinationValues)
            {

                _logger.Warning("combinationValue: " + combinationValue.Gtin);


                variant = new Variant();
                variant.Barcode = combinationValue.Gtin;
                variant.Id = combinationValue.Id.ToString();

                var pictureUrl = _pictureService.GetPictureUrl(combinationValue.PictureId);
                variant.ImageUrl = pictureUrl;
                variant.DimensionValues = new DimensionValues();

                XmlDocument attributesXml = new XmlDocument();
                attributesXml.LoadXml(combinationValue.AttributesXml);
                foreach(XmlNode node in attributesXml.DocumentElement)
                {
                    Dimension colorDimension = new Dimension();
                    Dimension sizeDimension = new Dimension();
                    if (Convert.ToInt32(node.FirstChild.FirstChild.InnerText) == colorAttributeId)
                    {                        
                        colorDimension.Id = "color";
                        colorDimension.Name = "Color";
                    }
                    else if(Convert.ToInt32(node.FirstChild.FirstChild.InnerText) == sizeAttributeId)
                    {
                        sizeDimension.Id = "size";
                        sizeDimension.Name = "Size";
                    }
                }
               
                variants.Add(variant);

            }

            //product.
            //    dimensions = new List<Dimension>();
            //List<ProductImage> productImages = ProductImage.FetchAllProductImages(productId);
            //List<Color> colors = Color.FetchAllColorsForProduct(productId, "");
            //List<Size> sizes = Size.FetchAllSizesForProduct(productId);

            //Dimension colorDimension = null;
            //bool hasColors = colors.Count > 0;
            //bool hasSizes = sizes.Count > 0;

            //if (hasColors)
            //{
            //    colorDimension = new Dimension();
            //    colorDimension.Id = "color";
            //    colorDimension.Name = "Color";
            //}

            //Dimension sizeDimension = null;
            //if (hasSizes)
            //{
            //    sizeDimension = new Dimension();
            //    sizeDimension.Id = "size";
            //    sizeDimension.Name = "Size";
            //}

            //List<Value> colorValues = new List<Value>();
            //List<Value> sizeValues = new List<Value>();
            //Value colorValue = null, sizeValue = null;
            //Variant variant = null;
            //List<string> usedColorIds = new List<string>();
            //List<string> usedSizeIds = new List<string>();
            //foreach (Product_Size_Color psc in pscs)
            //{
            //    string info = "Error in InCollection";
            //    try
            //    {
            //        ProductImage img = productImages.Where(i => i.ColorId == psc.ColorId).FirstOrDefault();
            //        if (img == null)
            //        {
            //            continue;
            //        }

            //        string imageUrl = Utils.ImageServerUrl + "Huge/" + img.ImageName;

            //        if (psc.InCollection)
            //        {
            //            if (hasColors)
            //            {
            //                info = "Error in building colorvalue";
            //                colorValue = new Value();
            //                colorValue.Id = psc.ColorId.ToString();
            //                colorValue.ImageUrl = imageUrl;
            //                colorValue.Name = colors.Where(c => c.Id == psc.ColorId).Select(c => c.DanishName).FirstOrDefault();

            //                if (!usedColorIds.Contains(colorValue.Id))
            //                {
            //                    colorValues.Add(colorValue);
            //                    usedColorIds.Add(colorValue.Id);
            //                }
            //            }

            //            if (hasSizes)
            //            {
            //                if (psc.SizeId == 0)
            //                {
            //                    continue;
            //                }

            //                info = "Error in building sizevalue";
            //                sizeValue = new Value();
            //                sizeValue.Id = psc.SizeId.ToString();
            //                sizeValue.Name = sizes.Where(s => s.Id == psc.SizeId).Select(s => s.DanishName).FirstOrDefault();

            //                if (!usedSizeIds.Contains(sizeValue.Id))
            //                {
            //                    sizeValues.Add(sizeValue);
            //                    usedSizeIds.Add(sizeValue.Id);
            //                }
            //            }

            //            if (hasColors || hasSizes)
            //            {
            //                info = "Error in building either colorvalue or sizevalue";
            //                variant = new Variant();
            //                variant.Barcode = psc.EAN;
            //                variant.Id = psc.ProductId + "-" + psc.ColorId + "-" + psc.SizeId;
            //                variant.ImageUrl = imageUrl;
            //                variant.DimensionValues = new DimensionValues();
            //                variant.DimensionValues.Color = hasColors ? colorValue.Id : null;
            //                variant.DimensionValues.Size = hasSizes ? sizeValue.Id : null;

            //                variants.Add(variant);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Exception newEx = new Exception(info, ex);
            //        ErrorLogging.SaveException(newEx, LoggingEnums.LogArea.AdminPOS);
            //        WebShopMail.SendErrorMail(newEx.Message, newEx.ToString());
            //    }


            //    if (hasColors)
            //    {
            //        colorDimension.Values = colorValues.ToArray();
            //        dimensions.Add(colorDimension);
            //    }

            //    if (hasSizes)
            //    {
            //        sizeDimension.Values = sizeValues.ToArray();
            //        dimensions.Add(sizeDimension);
            //    }
            //}

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

        //public void HandleEvent(EntityInsertedEvent<Product> eventMessage)
        //{
        //    _logger.Information("Handling product insert in POS Kaching Plugin");
        //    throw new NotImplementedException();
        //}
    }
}
