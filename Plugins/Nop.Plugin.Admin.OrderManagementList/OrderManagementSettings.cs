using Nop.Core.Configuration;

namespace Nop.Plugin.Admin.OrderManagementList
{
    public class OrderManagementSettings : ISettings
    {            
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

        /// <summary>
        /// "Labels/GLS/Status"
        /// </summary>
        public string FTPRemoteStatusFilePath { get; set; }

        /// <summary>
        /// 21: Pxx = Printer no. in GLS file
        /// http://www.gls.dk/information/Folder/InterLine%20Silent%20mode_EN.pdf
        /// </summary>
        public string FTPPrinterName { get; set; }

        /// <summary>
        /// Folder to keep all status files while searching for the right one for tracking number
        /// </summary>
        public string FTPTempFolder { get; set; }

        /// <summary>
        /// Number of seconds to wait in each retry for the status file
        /// </summary>
        public int GLSStatusFileWaitSeconds { get; set; }

        /// <summary>
        /// Number of retries to perform for status file
        /// </summary>
        public int GLSStatusFileRetries { get; set; }

        public bool DoCapture { get; set; }

        public bool DoSendEmails { get; set; }

        public bool ChangeOrderStatus { get; set; }

        public bool DoPrintLabel { get; set; }
    }
}
