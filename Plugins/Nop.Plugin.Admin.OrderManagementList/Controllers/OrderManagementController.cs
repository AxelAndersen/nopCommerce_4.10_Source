using GLSReference;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Admin.OrderManagementList.Domain;
using Nop.Plugin.Admin.OrderManagementList.Models;
using Nop.Plugin.Admin.OrderManagementList.Services;
using Nop.Plugin.Payments.QuickPayV10.Models;
using Nop.Plugin.Payments.QuickPayV10.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Nop.Plugin.Admin.OrderManagementList.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class OrderManagementController : BaseAdminController
    {
        #region Private variables
        private readonly ILogger _logger;
        private readonly OrderManagementSettings _settings;
        private readonly IOrderManagementService _orderManagementService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IReOrderService _reOrderService;
        private readonly IFTPService _ftpService;
        private readonly IGLSService _glsService;
        private readonly IQuickPayApiServices _quickPayService;
        private int _glsStatusFileRetries = 0;
        private string _trackingNumber;
        private bool _anyChangesDone; 
        #endregion

        public OrderManagementController(ILogger logger, 
                                         OrderManagementSettings orderManagementSettings, 
                                         ILocalizationService localizationService, 
                                         ISettingService settingService, 
                                         IOrderManagementService orderManagementService, 
                                         IFTPService ftpService, 
                                         IGLSService glsService, 
                                         IQuickPayApiServices quickPayService,
                                         IReOrderService reOrderService)
        {
            this._logger = logger;
            this._settings = orderManagementSettings;
            this._settingService = settingService;
            this._orderManagementService = orderManagementService;
            this._localizationService = localizationService;
            this._ftpService = ftpService;
            this._glsService = glsService;
            this._quickPayService = quickPayService;
            this._reOrderService = reOrderService;
        }

        #region Public methods
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult List(string searchphrase = "")
        {
            OrderManagementListModel model = new OrderManagementListModel();

            try
            {
                int markedProductId = 0;
                model.PresentationOrders = _orderManagementService.GetCurrentOrders(ref markedProductId, searchphrase);
                model.MarkedProductId = markedProductId;
                model.SearchPhrase = searchphrase;
                model.TotalCount = model.PresentationOrders.Count;
                model.TotalAmount = _orderManagementService.GetTotalAmount();                
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("List Order Management: " + inner.Message, ex);
                model.ErrorMessage = ex.ToString();
            }

            return View("~/Plugins/Nop.Plugin.Admin.OrderManagementList/Views/List.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            OrderManagementConfigurationModel model = null;
            try
            {
                model = GetBaseModel();
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure Order Management: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }
            return View("~/Plugins/Nop.Plugin.Admin.OrderManagementList/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(OrderManagementConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            try
            {
                _settings.ErrorMessage = model.ErrorMessage;

                _settings.FTPHost = model.FTPHost;
                _settings.FTPUsername = model.FTPUsername;
                if (string.IsNullOrEmpty(model.FTPPassword) == false)
                {
                    _settings.FTPPassword = model.FTPPassword;
                }
                _settings.FTPLocalFilePath = model.FTPLocalFilePath;
                _settings.FTPLocalFileName = model.FTPLocalFileName;
                _settings.FTPRemoteFolderPath = model.FTPRemoteFolderPath;
                _settings.FTPRemoteStatusFilePath = model.FTPRemoteStatusFilePath;
                _settings.FTPPrinterName = model.FTPPrinterName;
                _settings.FTPTempFolder = model.FTPTempFolder;
                _settings.GLSStatusFileRetries = model.GLSStatusFileRetries;
                _settings.GLSStatusFileWaitSeconds = model.GLSStatusFileWaitSeconds;
                _settings.DoCapture = model.DoCapture;
                _settings.DoSendEmails = model.DoSendEmails;
                _settings.ChangeOrderStatus = model.ChangeOrderStatus;
                _settings.DoPrintLabel = model.DoPrintLabel;
                _settings.DoCleanup = model.DoCleanup;
                _settings.DaysToKeepStatusFiles = model.DaysToKeepStatusFiles;

                _settingService.SaveSetting(_settings);
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("Configure Order Management: " + inner.Message, ex);
                model.ErrorMessage += "<br />" + inner.Message;
            }
            return Configure();
        }

        [HttpGet]
        [AuthorizeAdmin(false)]
        public IActionResult UpdateProductTakenAside(int orderId, int orderItemId, int productId, bool isTakenAside)
        {
            try
            {
                bool allwell = false;
                if (isTakenAside)
                {
                    string result = DoUpdateProductTakenAside(orderItemId, productId, isTakenAside, ref allwell);
                    if (allwell)
                    {
                        SuccessNotification(_localizationService.GetResource("Nop.Plugin.Admin.OrderManagementList.SuccessfullProductReady"));
                        return Json("Produkt taget fra " + result);
                    }
                    else
                    {
                        return Json("Error: " + result);
                    }
                }
                else
                {
                    string result = DoUpdateProductTakenAside(orderItemId, productId, isTakenAside, ref allwell);
                    if (allwell)
                    {
                        SuccessNotification(_localizationService.GetResource("Nop.Plugin.Admin.OrderManagementList.SuccessfullProductReady"));
                        return Json("Produkt IKKE taget fra " + result);
                    }
                    else
                    {
                        return Json("Error: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;

                _logger.Error(inner.Message, ex);
                return Json("Error: " + inner.Message);
            }
        }

        [HttpGet]
        [AuthorizeAdmin(false)]
        public IActionResult UpdateProductOrdered(int orderId, int orderItemId, int productId, bool isOrdered, int quantityToOrder)
        {
            try
            {
                bool allwell = false;
                if (isOrdered)
                {
                    string result = DoUpdateProductOrdered(orderItemId, productId, isOrdered, quantityToOrder, ref allwell);

                    if (allwell)
                    {
                        SuccessNotification(_localizationService.GetResource("Nop.Plugin.Admin.OrderManagementList.SuccessfullProductReady"));
                        return Json("Produkt bestilt" + result);
                    }
                    else
                    {
                        return Json("Error: " + result);
                    }
                }
                else
                {
                    string result = DoUpdateProductOrdered(orderItemId, productId, isOrdered, quantityToOrder, ref allwell);
                    if (allwell)
                    {
                        SuccessNotification(_localizationService.GetResource("Nop.Plugin.Admin.OrderManagementList.SuccessfullProductReady"));
                        return Json("Produkt IKKE bestilt" + result);
                    }
                    else
                    {
                        return Json("Error: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;

                _logger.Error(inner.Message, ex);
                return Json("Error: " + inner.Message);
            }
        }

        [HttpGet]
        [AuthorizeAdmin(false)]
        public IActionResult CompleteOrder(int orderId)
        {
            try
            {
                _anyChangesDone = false;
                AOOrder order = _orderManagementService.GetOrder(orderId);
                if (order == null)
                {
                    throw new ArgumentException("No order found with id: " + orderId);
                }

                if (string.IsNullOrEmpty(order.AuthorizationTransactionId))
                {
                    throw new ArgumentException("No payment id found with orderid: " + orderId);
                }

                HandleGLSLabel(order);

                Capture(order);

                SendMails(order);

                _glsStatusFileRetries = 0;
                SetTrackingNumber(order);

                ChangeOrderStatus(orderId);                

                if (_anyChangesDone == false)
                {
                    return Json("Warning: No changes done. Maybe change settings for this plugin.");
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;

                _logger.Error(ex.Message, ex);
                return Json("Error: " + ex.Message);
            }
            return Json("Done");
        }

        private void CleanupGLSStatusFiles()
        {
            if (_settings.DoCleanup)
            {
                _ftpService.CleanupGLSStatusFiles(_settings.FTPRemoteStatusFilePath, _settings.DaysToKeepStatusFiles);

                string[] files = Directory.GetFiles(_settings.FTPTempFolder);

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddDays(-_settings.DaysToKeepStatusFiles))
                    {
                        fi.Delete();
                    }
                }
            }
        }
        #endregion

        #region Private methods
        private void ChangeOrderStatus(int orderId)
        {
            if (_settings.ChangeOrderStatus)
            {
                _orderManagementService.ChangeOrderStatus(orderId);
                _anyChangesDone = true;
            }
        }

        private void SendMails(AOOrder order)
        {
            if (_settings.DoSendEmails)
            {
                _orderManagementService.SendShipmentMail(order);
                _anyChangesDone = true;
            }
        }

        private void Capture(AOOrder order)
        {
            if (_settings.DoCapture)
            {
                PaymentApiStatus paymentApiStatus = _quickPayService.GetPayment(order.AuthorizationTransactionId);
                if (paymentApiStatus.Payment == null || paymentApiStatus.HttpResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new ArgumentException("No payment found with orderid: " + order.Id + " and AuthorizationTransactionId: " + order.AuthorizationTransactionId);
                }

                PaymentApiStatus captureStatus = _quickPayService.CapturePayment(order.AuthorizationTransactionId, Convert.ToInt32(order.TotalOrderAmount));

                if (captureStatus.HttpResponse.IsSuccessStatusCode == false)
                {
                    throw new ArgumentException("Error capturing money on orderid: " + order.Id + ", Error: " + captureStatus.HttpResponse.ReasonPhrase);
                }

                _anyChangesDone = true;
            }
        }

        private void SetTrackingNumber(AOOrder order)
        {
            if (_settings.DoPrintLabel)
            {
                Thread.Sleep(_settings.GLSStatusFileWaitSeconds * 1000);
                _trackingNumber = _ftpService.GetTrackingNumber(_settings.FTPTempFolder, _settings.FTPRemoteStatusFilePath, order.Id);

                if (string.IsNullOrEmpty(_trackingNumber))
                {
                    if (_glsStatusFileRetries < _settings.GLSStatusFileRetries)
                    {
                        _glsStatusFileRetries++;
                        SetTrackingNumber(order);
                    }

                    CleanupGLSStatusFiles();

                    if (string.IsNullOrEmpty(_trackingNumber))
                    {
                        // This extra check is important!
                        throw new ArgumentException("No tracking number found for orderid: " + order.Id + ". Maybe number of retries should be increased.");
                    }
                }

                _orderManagementService.SetTrackingNumberOnShipment(order.Shipment, _trackingNumber);
                _anyChangesDone = true;
            }
        }

        private void HandleGLSLabel(AOOrder order)
        {
            if (_settings.DoPrintLabel)
            {
                _ftpService.Initialize(
                            _settings.FTPHost,
                            _settings.FTPUsername,
                            _settings.FTPPassword);

                string localFilepath = _settings.FTPLocalFilePath + "\\" + _settings.FTPLocalFileName;

                System.IO.File.WriteAllText(localFilepath, CreateSingleLine(order), Encoding.UTF8);

                _ftpService.SendFile(localFilepath, _settings.FTPRemoteFolderPath + "/" + _settings.FTPLocalFileName);

                _anyChangesDone = true;
            }
        }

        private string CreateSingleLine(AOOrder order)
        {
            string glsShopnumber = "";
            if (order.ShippingInfo.Contains("("))
            {
                glsShopnumber = order.ShippingInfo.Substring(order.ShippingInfo.IndexOf("(") + 1);
                glsShopnumber = glsShopnumber.Substring(0, glsShopnumber.IndexOf(")"));
            }

            PakkeshopData pakkeshopData = _glsService.GetParcelShopData(glsShopnumber);
            StringBuilder sb = new StringBuilder();

            sb.Append("\"" + order.Id.ToString() + "\"");               // 1 Order number            
            sb.Append(",\"" + pakkeshopData.CompanyName + "\"");        // 2 Consignee name (Name of parcelshop)
            sb.Append(",\"" + order.CustomerInfo + "\"");               // 3 Recipient address
            sb.Append(",\"" + pakkeshopData.Number + "\"");             // 4 Parcelshop number
            sb.Append(",\"" + pakkeshopData.ZipCode + "\"");            // 5 Zipcode of recipient        
            sb.Append(",\"" + pakkeshopData.CityName + "\"");           // 6 Postal district of recipient 
            sb.Append(",\"" + pakkeshopData.CountryCode + "\"");        // 7 Country of recipient
            sb.Append(",\"" + DateTime.Now.ToString("dd-MM-yy") + "\"");// 8 Date 
            sb.Append(",\"1\"");                                        // 9 Parcel weight 
            sb.Append(",\"1\"");                                        // 10 Number of parcels 
            sb.Append(",\"\"");                                         // 11 COD Amount (Order amount?)
            sb.Append(",\"\"");                                         // 12 Parcel value amount (Total parcel value)
            sb.Append(",\"A\"");                                        // 13 Parcel type
            sb.Append(",\"Z\"");                                        // 14 Shipment type             
            sb.Append(",\"" + order.UserName + "\"");                   // 15 Name of recipient to pick up parcel            
            sb.Append(",\"\"");                                         // 16 Customer note (not the one for friliv.dk)
            sb.Append(",\"90022\"");                                    // 17 Customer number (vores GLS kundenummer)
            sb.Append(",\"" + order.CustomerEmail + "\"");              // 18 Customer mail address
            sb.Append(",\"" + order.PhoneNumber + "\"");                // 19 Customer mobile number
            sb.Append(",\"\"E");                                        // 20 Notification: E = Email
            sb.Append(",\"\"" + _settings.FTPPrinterName + "\"");       // 21 Pxx = Printer no
            sb.Append(",\"\"");                                         // 22
            sb.Append(",\"\"");                                         // 23
            sb.Append(",\"\"");                                         // 24
            sb.Append(",\"\"");                                         // 25
            sb.Append(",\"\"");                                         // 26
            sb.Append(",\"\"");                                         // 27
            sb.Append(",\"\"");                                         // 28
            sb.Append(",\"\"");                                         // 29

            string info = sb.ToString();
            return info;
        }

        private string DoUpdateProductTakenAside(int orderItemId, int productId, bool isTakenAside, ref bool allwell)
        {
            string errorMessage = "";
            _orderManagementService.SetProductIsTakenAside(orderItemId, productId, isTakenAside, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                allwell = true;
                return "";
            }
            else
            {
                allwell = false;
                return _localizationService.GetResource("Nop.Plugin.Admin.OrderManagementList.FailedUpdate") + ": " + errorMessage;
            }
        }

        private string DoUpdateProductOrdered(int orderItemId, int productId, bool isOrdered, int quantityToOrder, ref bool allwell)
        {
            string errorMessage = "";
            _orderManagementService.SetProductOrdered(orderItemId, productId, isOrdered, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (isOrdered)
                {
                    _reOrderService.RemoveFromReOrderList(quantityToOrder, orderItemId);
                }
                else
                {
                    _reOrderService.ReAddToReOrderList(orderItemId);
                }
                allwell = true;
                
                return "";
            }
            else
            {
                allwell = false;
                return _localizationService.GetResource("Nop.Plugin.Admin.OrderManagementList.FailedUpdate: " + errorMessage);
            }
        }

        private OrderManagementConfigurationModel GetBaseModel()
        {
            return new OrderManagementConfigurationModel
            {
                ErrorMessage = _settings.ErrorMessage,
                FTPHost = _settings.FTPHost,
                FTPUsername = _settings.FTPUsername,
                FTPPassword = _settings.FTPPassword,
                FTPLocalFilePath = _settings.FTPLocalFilePath,
                FTPLocalFileName = _settings.FTPLocalFileName,
                FTPRemoteFolderPath = _settings.FTPRemoteFolderPath,
                FTPPrinterName = _settings.FTPPrinterName,
                FTPRemoteStatusFilePath = _settings.FTPRemoteStatusFilePath,
                FTPTempFolder = _settings.FTPTempFolder,
                GLSStatusFileRetries = _settings.GLSStatusFileRetries,
                GLSStatusFileWaitSeconds = _settings.GLSStatusFileWaitSeconds,
                DoCapture = _settings.DoCapture,
                DoSendEmails = _settings.DoSendEmails,
                ChangeOrderStatus = _settings.ChangeOrderStatus,
                DoPrintLabel = _settings.DoPrintLabel,
                DoCleanup = _settings.DoCleanup,
                DaysToKeepStatusFiles = _settings.DaysToKeepStatusFiles
            };
        } 
        #endregion
    }
}