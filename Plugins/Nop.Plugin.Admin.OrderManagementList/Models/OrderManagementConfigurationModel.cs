using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Admin.OrderManagementList.Models
{
    public class OrderManagementConfigurationModel
    {
        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.ErrorMessage")]
        public string ErrorMessage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.FTPHost")]
        public string FTPHost { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.FTPUsername")]
        public string FTPUsername { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.FTPPassword")]
        [DataType(DataType.Password)]
        public string FTPPassword { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.FTPLocalFilePath")]
        /// <summary>
        /// "C:\Temp"
        /// </summary>
        public string FTPLocalFilePath { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.FTPLocalFileName")]
        /// <summary>
        /// "GLS-Label-Info.txt"
        /// </summary>
        public string FTPLocalFileName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.FTPRemoteFolderPath")]
        /// <summary>
        /// "Labels/GLS"
        /// </summary>
        public string FTPRemoteFolderPath { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.FTPRemoteStatusFilePath")]
        /// <summary>
        /// "Labels/GLS/Status"
        /// </summary>
        public string FTPRemoteStatusFilePath { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.FTPTempFolder")]
        /// <summary>
        /// Folder to keep all status files while searching for the right one for tracking number
        /// </summary>
        public string FTPTempFolder { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.FTPPrinterName")]
        /// <summary>
        /// 21: Pxx = Printer no. in GLS file
        /// http://www.gls.dk/information/Folder/InterLine%20Silent%20mode_EN.pdf
        /// </summary>
        public string FTPPrinterName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.GLSStatusFileWaitSeconds")]
        /// <summary>
        /// Number of seconds to wait in each retry for the status file
        /// </summary>
        public int GLSStatusFileWaitSeconds { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.GLSStatusFileRetries")]
        /// <summary>
        /// Number of retries to perform for status file
        /// </summary>
        public int GLSStatusFileRetries { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.DoPrintLabel")]
        public bool DoPrintLabel { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.DoCapture")]
        public bool DoCapture { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.DoSendEmails")]
        public bool DoSendEmails { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.ChangeOrderStatus")]
        public bool ChangeOrderStatus { get; set; }        
    }
}
