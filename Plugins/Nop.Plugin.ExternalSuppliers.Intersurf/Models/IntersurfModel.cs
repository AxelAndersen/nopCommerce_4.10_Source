using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Models
{
    public class IntersurfModel
    {        
        [Display(Name = "Endpoint address")]
        public string EndpointAddress { get; set; }

        [Display(Name = "User name")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "CSV file name")]
        public string CSVFileName { get; set; }

        public string ErrorMessage { get; set; }
    }
}
