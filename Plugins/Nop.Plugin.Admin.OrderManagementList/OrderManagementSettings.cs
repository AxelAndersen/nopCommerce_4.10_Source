using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList
{
    public class OrderManagementSettings : ISettings
    {
        public bool ListActive { get; set; }
        public string WelcomeMessage { get; set; }
        public string ErrorMessage { get; set; }

        public string FTPHost { get; set; }
        public string FTPUsername { get; set; }
        public string FTPPassword { get; set; }

        /// <summary>
        /// "C:\Temp"
        /// </summary>
        public string FTPLocalFilePath { get; set; }

        /// <summary>
        /// "GLS-Label-Info.txt"
        /// </summary>
        public string FTPLocalFileName { get; set; }

        /// <summary>
        /// "Labels/GLS"
        /// </summary>
        public string FTPRemoteFolderPath { get; set; }
    }
}
