using GLSReference;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.GLS.Components;
using Nop.Plugin.Shipping.GLS.Settings;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly ILogger _logger;
        private readonly StringBuilder _traceMessages;

        public GLSComputationMethod(IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService, ICountryService countryService, GLSSettings glsSettings, ILogger logger)
        {
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._countryService = countryService;
            this._glsSettings = glsSettings;
            this._logger = logger;

            this._traceMessages = new StringBuilder();
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
            var response = new GetShippingOptionResponse();

            if (_glsSettings.Tracing)
                _traceMessages.AppendLine("Ready to validate shipping input");            

            bool validInput = ValidateShippingInfo(getShippingOptionRequest, ref response);
            if(validInput == false && response.Errors != null && response.Errors.Count > 0)
            {
                return response;
            }

            try
            {
                if (_glsSettings.Tracing)
                    _traceMessages.Append("\r\nReady to prapare GLS call");

                wsShopFinderSoapClient client = new wsShopFinderSoapClient(EndpointConfiguration.wsShopFinderSoap12, _glsSettings.EndpointAddress);
                string zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
                string street = getShippingOptionRequest.ShippingAddress.Address1;               
                string glsCountryCode = Associations.GLSCountryCode[getShippingOptionRequest.ShippingAddress.Country.ThreeLetterIsoCode];

                if (_glsSettings.Tracing)
                    _traceMessages.Append("\r\nReady to call GLS at: '" + _glsSettings.EndpointAddress + "'");

                List<PakkeshopData> parcelShops = null;
                try
                {
                    // First try to find a number of shops near the address
                    parcelShops = client.GetParcelShopDropPointAsync(street, zip, glsCountryCode, _glsSettings.AmountNearestShops).Result.parcelshops.ToList();
                }
                catch { }

                if (parcelShops == null || parcelShops.Count == 0)
                {
                    try
                    {
                        // If any errors or no shop found, try to find shops from only zip code
                        parcelShops = client.GetParcelShopsInZipcodeAsync(zip, glsCountryCode).Result.GetParcelShopsInZipcodeResult.ToList();                        
                    }
                    catch (Exception ex)
                    {
                        response.AddError(ex.ToString());
                        if (_glsSettings.Tracing)
                            _traceMessages.Append("\r\nError finding parcelshops: " + ex.ToString());
                    }
                }

                if(parcelShops == null || parcelShops.Count == 0)
                {
                    response.AddError($"GLS Service could not find any shops with the given information: {street} {zip}");
                    return response;
                }

                if (_glsSettings.Tracing && parcelShops != null && parcelShops.Count > 0)
                {
                    _traceMessages.Append("\r\n" + parcelShops.Count + " parcelshops found");
                }

                foreach (var parcelShop in parcelShops)
                {
                    ShippingOption shippingOption = new ShippingOption()
                    {
                        Name = BuildGLSName(parcelShop), 
                        Description = BuildGLSDescription(parcelShop),                        
                        ShippingRateComputationMethodSystemName = "GLS",
                        Rate = _glsSettings.SwedishRate
                    };
                    
                    response.ShippingOptions.Add(shippingOption);                    
                }

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

                if (response.ShippingOptions.Any())
                {
                    response.Errors.Clear();
                }
            }           
            catch (Exception exc)
            {
                while (exc.InnerException != null) exc = exc.InnerException;
                response.AddError($"GLS Service is currently unavailable, try again later. {exc.ToString()}");

                if (_glsSettings.Tracing)
                    _traceMessages.Append($"\r\nGLS Service is currently unavailable, try again later. {exc.ToString()}");
            }
            finally
            {
                if (_glsSettings.Tracing && _traceMessages.Length > 0)
                {
                    var shortMessage =
                        $"GLS Shipping Options for customer {getShippingOptionRequest.Customer.Email}.  {getShippingOptionRequest.Items.Count} item(s) in cart";
                    _logger.Information(shortMessage, new Exception(_traceMessages.ToString()), getShippingOptionRequest.Customer);
                }
            }

            return response;
        }

        private string BuildGLSDescription(PakkeshopData parcelShop)
        {
            string desc = "";

            //if (string.IsNullOrEmpty(parcelShop.Telephone) == false)
            //{
            //    desc += " Phone: " + parcelShop.Telephone;
            //}

            //if (string.IsNullOrEmpty(parcelShop.Latitude) == false)
            //{
            //    desc += " Latitude: " + parcelShop.Latitude;
            //}

            //if (string.IsNullOrEmpty(parcelShop.Longitude) == false)
            //{
            //    desc += " Longitude: " + parcelShop.Longitude;
            //}

            //if (parcelShop.DistanceMetersAsTheCrowFlies > 0)
            //{
            //    desc += " Distance Meters As The Crow Flies: " + parcelShop.DistanceMetersAsTheCrowFlies;
            //}

            return desc;
        }

        private string BuildGLSName(PakkeshopData parcelShop)
        {
            string name = parcelShop.CompanyName + " (" + parcelShop.Number + ")<br />"
                + parcelShop.Streetname + "<br />"
                + parcelShop.ZipCode + " " + parcelShop.CityName + "<br />";            

            return name;
        }

        private bool ValidateShippingInfo(GetShippingOptionRequest getShippingOptionRequest, ref GetShippingOptionResponse response)
        {
            if (getShippingOptionRequest == null)
            {
                throw new ArgumentNullException(nameof(getShippingOptionRequest));            
            }

            if (getShippingOptionRequest.Items == null)
            {
                response.AddError("No shipment items");
                return false;
            }

            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return false;
            }

            if (getShippingOptionRequest.ShippingAddress.Country == null)
            {
                response.AddError("Shipping country is not set");
                return false;
            }

            if (getShippingOptionRequest.CountryFrom == null)
            {
                response.AddError("From country is not set");
                return false;
            }

            if (!Associations.GLSCountryCode.ContainsKey(getShippingOptionRequest.ShippingAddress.Country.ThreeLetterIsoCode))
            {
                response.AddError($"Zip iso code '{getShippingOptionRequest.ShippingAddress.Country.ThreeLetterIsoCode}' not found in asscociations");
                return false;
            }

            if (string.IsNullOrEmpty(getShippingOptionRequest.ShippingAddress.ZipPostalCode))
            {
                response.AddError("Zipcode not set");
                return false;
            }

            return true;
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
