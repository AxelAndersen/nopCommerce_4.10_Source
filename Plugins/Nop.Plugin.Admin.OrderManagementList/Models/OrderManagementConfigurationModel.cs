using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList.Models
{
    public class OrderManagementConfigurationModel
    {
        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.ListActive")]
        public bool ListActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Admin.OrderManagementList.WelcomeMessage")]
        public string WelcomeMessage { get; set; }

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
    }
}
