using Nop.Core.Configuration;

namespace Nop.Plugin.Admin.OrderManagementList
{
    public class OrderManagementSettings : ISettings
    {
        public bool ListActive { get; set; }        
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
        /// 21: Pxx = Printer no. in GLS file
        /// http://www.gls.dk/information/Folder/InterLine%20Silent%20mode_EN.pdf
        /// </summary>
        public string FTPPrinterName { get; set; }
    }
}
