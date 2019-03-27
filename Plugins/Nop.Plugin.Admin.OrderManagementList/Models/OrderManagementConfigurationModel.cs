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
    }
}
