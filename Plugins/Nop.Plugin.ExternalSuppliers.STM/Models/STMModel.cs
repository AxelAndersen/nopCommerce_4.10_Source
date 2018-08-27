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

        public string ErrorMessage { get; set; }
    }
}
