using Nop.Core.Domain.Media;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Media;
using System;

namespace Nop.Plugin.POS.Kaching
{
    public class POSKachingEventConsumer : IConsumer<EntityUpdatedEvent<Core.Domain.Catalog.Product>>
    {
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISettingService _settingService;
        private readonly POSKachingSettings _kachingSettings;

        private readonly MediaSettings _mediaSettings;


        public POSKachingEventConsumer(ILogger logger, IProductService productService, IPictureService pictureService, IProductAttributeService productAttributeService, ISettingService settingService, POSKachingSettings kachingSettings)
        {
            this._logger = logger;
            this._productService = productService;
            this._pictureService = pictureService;
            this._productAttributeService = productAttributeService;
            this._settingService = settingService;
            this._kachingSettings = kachingSettings;
        }

        public void HandleEvent(EntityUpdatedEvent<Core.Domain.Catalog.Product> eventMessage)
        {
            _logger.Information("Handling product update in POS Kaching Plugin");

            Core.Domain.Catalog.Product product = null;

            try
            {
                product = GetProduct(eventMessage);
                POSKachingService service = new POSKachingService(_logger, _kachingSettings, _settingService, _pictureService, _productAttributeService);
                var json = service.BuildJSONString(product);
                
                service.SaveProduct(json);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("HandleEvent POS Kaching: " + inner.Message, ex); 
            }
        }

        private Core.Domain.Catalog.Product GetProduct(EntityUpdatedEvent<Core.Domain.Catalog.Product> eventMessage)
        {
            Core.Domain.Catalog.Product product = _productService.GetProductById(eventMessage.Entity.Id);

            if (product == null || product.Deleted)
            {
                throw new ArgumentException("No Product active found with id: " + eventMessage.Entity.Id);
            }

            return product;
        }       
    }
}
