using AO.Services.Extensions;
using GLSReference;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.GLS.Models;
using Nop.Plugin.Shipping.GLS.Services;
using Nop.Plugin.Shipping.GLS.Settings;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static GLSReference.wsShopFinderSoapClient;

namespace Nop.Plugin.Shipping.GLS
{
    public class GLSComputationMethod : BasePlugin, IMiscPlugin, IShippingRateComputationMethod
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly GLSSettings _glsSettings;
        private readonly ILogger _logger;
        private readonly StringBuilder _traceMessages;
        private readonly IGLSService _glsService;
        private readonly ICurrencyService _currencyService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;

        public GLSComputationMethod(IWebHelper webHelper, ISettingService settingService, GLSSettings glsSettings, ILogger logger, IGLSService glsService, ICurrencyService currencyService, IWorkContext workContext, IOrderTotalCalculationService orderTotalCalculationService)
        {
            this._webHelper = webHelper;
            this._settingService = settingService;       
            this._glsSettings = glsSettings;
            this._logger = logger;
            this._glsService = glsService;
            this._currencyService = currencyService;
            this._workContext = workContext;
            this._orderTotalCalculationService = orderTotalCalculationService;

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

        public object EndsWith { get; private set; }

        #endregion

        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return null;
        }

        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {            
            var response = new GetShippingOptionResponse();

            if (_glsSettings.Tracing)
                _traceMessages.AppendLine("Ready to validate shipping input");

            AOGLSCountry glsCountry = null;
            bool validInput = ValidateShippingInfo(getShippingOptionRequest, ref response, ref glsCountry);
            if(validInput == false && response.Errors != null && response.Errors.Count > 0)
            {
                return response;
            }

            try
            {
                if (_glsSettings.Tracing)
                    _traceMessages.Append("\r\nReady to prapare GLS call");

                List<PakkeshopData> parcelShops = null;

                if (glsCountry.SupportParcelShop)
                {
                    wsShopFinderSoapClient client = new wsShopFinderSoapClient(EndpointConfiguration.wsShopFinderSoap12, _glsSettings.EndpointAddress);
                    string zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
                    string street = getShippingOptionRequest.ShippingAddress.Address1;

                    if (_glsSettings.Tracing)
                        _traceMessages.Append("\r\nReady to call GLS at: '" + _glsSettings.EndpointAddress + "'");
                    
                    try
                    {
                        // First try to find a number of shops near the address
                        parcelShops = client.GetParcelShopDropPointAsync(street, zip, glsCountry.TwoLetterGLSCode, _glsSettings.AmountNearestShops).Result.parcelshops.ToList();
                    }
                    catch (Exception ex)
                    {
                        if (_glsSettings.Tracing)
                            _traceMessages.Append("\r\nError finding parcelshops: " + ex.ToString());
                    }

                    if (parcelShops == null || parcelShops.Count == 0)
                    {
                        try
                        {
                            // If any errors or no shop found, try to find shops from only zip code
                            parcelShops = client.GetParcelShopsInZipcodeAsync(zip, glsCountry.TwoLetterGLSCode).Result.GetParcelShopsInZipcodeResult.ToList();
                        }
                        catch (Exception ex)
                        {
                            if (_glsSettings.Tracing)
                                _traceMessages.Append("\r\nError finding parcelshops: " + ex.ToString());
                        }
                    }

                    if (_glsSettings.Tracing && parcelShops != null && parcelShops.Count > 0)
                    {
                        _traceMessages.Append("\r\n" + parcelShops.Count + " parcelshops found");
                    }

                    if (parcelShops != null && parcelShops.Count > 0)
                    {
                        foreach (var parcelShop in parcelShops)
                        {
                            ShippingOption shippingOption = new ShippingOption()
                            {
                                Name = BuildGLSName(parcelShop),
                                Description = "",
                                ShippingRateComputationMethodSystemName = "GLS",
                                Rate = GetRate(glsCountry.ShippingPrice_0_1)
                            };

                            response.ShippingOptions.Add(shippingOption);
                        }
                    }
                }

                if (parcelShops == null || parcelShops.Count == 0)
                {
                    Address address = getShippingOptionRequest.ShippingAddress;
                    ShippingOption shippingOption = new ShippingOption()
                    {
                        Name = address.FirstName + " " + address.LastName,
                        Description = BuildDescription(address),
                        ShippingRateComputationMethodSystemName = "GLS",
                        Rate = GetRate(glsCountry.ShippingPrice_0_1)
                    };

                    response.ShippingOptions.Add(shippingOption);
                }
                
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

        private string BuildDescription(Address address)
        {
            string description = address.Address1;

            if (string.IsNullOrEmpty(address.ZipPostalCode) == false)
            {
                description += ", " + address.ZipPostalCode;
            }

            if (string.IsNullOrEmpty(address.City) == false)
            {
                description += " " + address.City;
            }

            if (string.IsNullOrEmpty(address.Country?.Name) == false)
            {
                description += ", " + address.Country.Name;
            }

            return description;
        }

        private decimal GetRate(decimal price)
        {
            var cartItems = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            _orderTotalCalculationService.GetShoppingCartSubTotal(cartItems, true, out var orderSubTotalDiscountAmount, out var orderSubTotalAppliedDiscounts, out var subTotalWithoutDiscountBase, out var _);

            if (_glsSettings.FreeShippingLimit > 0 && subTotalWithoutDiscountBase > _glsSettings.FreeShippingLimit)
            {
                return 0;
            }

            Currency eurCurrency = _currencyService.GetCurrencyByCode("EUR", true);
            decimal storePrice = _currencyService.ConvertToPrimaryStoreCurrency(price, eurCurrency);

            return storePrice.AdjustPriceToPresentation(_glsSettings.PricesEndsWith);
        }

        private string BuildGLSName(PakkeshopData parcelShop)
        {
            string name = parcelShop.CompanyName + " (" + parcelShop.Number + ")<br />"
                + parcelShop.Streetname + "<br />"
                + parcelShop.ZipCode + " " + parcelShop.CityName + "<br />";            

            return name;
        }

        private bool ValidateShippingInfo(GetShippingOptionRequest getShippingOptionRequest, ref GetShippingOptionResponse response, ref AOGLSCountry glsCountry)
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

            glsCountry = _glsService.GetCountryByThreeLetterCode(getShippingOptionRequest.ShippingAddress.Country.ThreeLetterIsoCode);
            if (glsCountry == null)
            {
                response.AddError("Not possible to send to " + getShippingOptionRequest.ShippingAddress.Country.Name + " (" + getShippingOptionRequest.ShippingAddress.Country.ThreeLetterIsoCode + ")");                
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
