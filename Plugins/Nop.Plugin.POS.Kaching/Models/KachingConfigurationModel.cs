﻿using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class KachingConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Nop.Plugin.POS.Kaching.KaChingHost")]
        public string POSKaChingHost { get; set; }

        [NopResourceDisplayName("Nop.Plugin.POS.Kaching.POSKaChingId")]        
        public string POSKaChingId { get; set; }

        [NopResourceDisplayName("Nop.Plugin.POS.Kaching.POSKaChingAccountToken")]
        public string POSKaChingAccountToken { get; set; }

        [NopResourceDisplayName("Nop.Plugin.POS.Kaching.POSKaChingAPIToken")]
        public string POSKaChingAPIToken { get; set; }

        [NopResourceDisplayName("Nop.Plugin.POS.Kaching.POSKaChingImportQueueName")]
        public string POSKaChingImportQueueName { get; set; }

        [Display(Name ="Kaching alive")]
        public string KachingAlive { get; set; }
        public string KachingAliveValue { get; set; }
        public string KachingIsDead { get; set; }
        public string ProductsTransferred { get; set; }
        public string ErrorMessage { get; set; }
    }
}
