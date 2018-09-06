using Nop.Core.Configuration;
using System;

namespace Nop.Plugin.ExternalSuppliers.Bonvita
{
    public class BonvitaSettings : ISettings
    {
        /// <summary>
        /// 85.235.231.188
        /// </summary>
        public string FtpHost { get; set; }

        /// <summary>
        /// axel@friliv.dk
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Friliv2000!
        /// </summary>
        public string Pass { get; set; }

        /// <summary>
        /// bonvitastocklist.csv
        /// </summary>
        public string CSVFileName { get; set; }
    }
}
