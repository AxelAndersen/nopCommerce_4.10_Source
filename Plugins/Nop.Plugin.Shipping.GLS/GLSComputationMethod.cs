using GLSReference;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.GLS.Components;
using Nop.Plugin.Shipping.GLS.Settings;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using System;
using System.Linq;
using static GLSReference.wsShopFinderSoapClient;

namespace Nop.Plugin.Shipping.GLS
{
    public class GLSComputationMethod : BasePlugin, IMiscPlugin, IShippingRateComputationMethod
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICountryService _countryService;
        private readonly GLSSettings _glsSettings;

        public GLSComputationMethod(IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService, ICountryService countryService, GLSSettings glsSettings)
        {
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._countryService = countryService;
            this._glsSettings = glsSettings;
        }

        #region Properties
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get { return ShippingRateComputationMethodType.Realtime; }
        }

        public IShipmentTracker ShipmentTracker
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        IShipmentTracker IShippingRateComputationMethod.ShipmentTracker
        {
            get
            {
                return null;
            }
        }

        #endregion

        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return 0;
        }

        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
            {
                throw new ArgumentNullException(nameof(getShippingOptionRequest));
            }

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null)
            {
                response.AddError("No shipment items");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress.Country == null)
            {
                response.AddError("Shipping country is not set");
                return response;
            }

            if (getShippingOptionRequest.CountryFrom == null)
            {
                response.AddError("From country is not set");
                return response;
            }

            try
            {

                wsShopFinderSoapClient client = new wsShopFinderSoapClient(EndpointConfiguration.wsShopFinderSoap12, _glsSettings.EndpointAddress);
                string zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
                if (string.IsNullOrEmpty(zip))
                {
                    response.AddError("Zipcode not set");
                    return response;
                }

                if (!Associations.GLSCountryCode.ContainsKey(getShippingOptionRequest.ShippingAddress.Country.ThreeLetterIsoCode))
                {
                    response.AddError($"Zip iso code '{getShippingOptionRequest.ShippingAddress.Country.ThreeLetterIsoCode}' not found in asscociations");
                    return response;
                }
                string glsCountryCode = Associations.GLSCountryCode[getShippingOptionRequest.ShippingAddress.Country.ThreeLetterIsoCode];


                var shops = client.GetParcelShopsInZipcodeAsync(zip, glsCountryCode).Result;


                //var requestString = CreateRequest(_glsSettings.AccessKey, _glsSettings.Username, _glsSettings.Password, getShippingOptionRequest,
                //    _upsSettings.CustomerClassification, _upsSettings.PickupType, _upsSettings.PackagingType, false);
                //if (_upsSettings.Tracing)
                //    _traceMessages.AppendLine("Request:").AppendLine(requestString);

                //var responseXml = DoRequest(_upsSettings.Url, requestString);
                //if (_upsSettings.Tracing)
                //    _traceMessages.AppendLine("Response:").AppendLine(responseXml);

                //var error = "";
                //var shippingOptions = ParseResponse(responseXml, ref error);
                //if (string.IsNullOrEmpty(error))
                //{
                //    foreach (var shippingOption in shippingOptions)
                //    {
                //        if (!shippingOption.Name.ToLower().StartsWith("ups"))
                //            shippingOption.Name = $"UPS {shippingOption.Name}";
                //        shippingOption.Rate += _upsSettings.AdditionalHandlingCharge;
                //        response.ShippingOptions.Add(shippingOption);
                //    }
                //}
                //else
                //{
                //    response.AddError(error);
                //}

                ////Saturday delivery
                //if (_upsSettings.CarrierServicesOffered.Contains("[sa]"))
                //{
                //    requestString = CreateRequest(_upsSettings.AccessKey, _upsSettings.Username, _upsSettings.Password, getShippingOptionRequest,
                //        _upsSettings.CustomerClassification, _upsSettings.PickupType, _upsSettings.PackagingType, true);
                //    if (_upsSettings.Tracing)
                //        _traceMessages.AppendLine("Request:").AppendLine(requestString);

                //    responseXml = DoRequest(_upsSettings.Url, requestString);
                //    if (_upsSettings.Tracing)
                //        _traceMessages.AppendLine("Response:").AppendLine(responseXml);

                //    error = string.Empty;
                //    var saturdayDeliveryShippingOptions = ParseResponse(responseXml, ref error);
                //    if (string.IsNullOrEmpty(error))
                //    {
                //        foreach (var shippingOption in saturdayDeliveryShippingOptions)
                //        {
                //            shippingOption.Name =
                //                $"{(shippingOption.Name.ToLower().StartsWith("ups") ? string.Empty : "UPS ")}{shippingOption.Name} - Saturday Delivery";
                //            shippingOption.Rate += _upsSettings.AdditionalHandlingCharge;
                //            response.ShippingOptions.Add(shippingOption);
                //        }
                //    }
                //    else
                //        response.AddError(error);
                //}

                //if (response.ShippingOptions.Any())
                //    response.Errors.Clear();
            }
            catch (Exception exc)
            {
                response.AddError($"GLS Service is currently unavailable, try again later. {exc.Message}");
            }
            finally
            {
                //if (_upsSettings.Tracing && _traceMessages.Length > 0)
                //{
                //    var shortMessage =
                //        $"UPS Get Shipping Options for customer {getShippingOptionRequest.Customer.Email}.  {getShippingOptionRequest.Items.Count} item(s) in cart";
                //    _logger.Information(shortMessage, new Exception(_traceMessages.ToString()), getShippingOptionRequest.Customer);
                //}
            }

            return response;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        /// 
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/GLS/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new GLSSettings());

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<GLSSettings>();

            base.Uninstall();
        }
    }
}
