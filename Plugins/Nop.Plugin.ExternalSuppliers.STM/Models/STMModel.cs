using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Plugin.ExternalSuppliers.STM.Models
{
    public class STMModel
    {        
        [Display(Name = "Endpoint address")]
        public string EndpointAddress { get; set; }

        [Display(Name = "Minimum stock count")]
        public string MinimumStockCount { get; set; }

        /// <summary>
        /// This is to know how far in the list we have reached.
        /// We cant take everything at once, due to timeouts.
        /// </summary>
        [Display(Name = "Number to Skip")]
        public int SkipNumber { get; set; }

        public string ErrorMessage { get; set; }
    }
}
