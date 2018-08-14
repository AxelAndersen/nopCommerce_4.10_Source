using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching
{
    public class POSKachingSettings : ISettings
    {
        public string POSKaChingHost { get; set; }
        public string POSKaChingId { get; set; }
        public string POSKaChingAccountToken { get; set; }
        public string POSKaChingAPIToken { get; set; }
        public string POSKaChingImportQueueName { get; set; }
    }
}
