using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Shipping.GLS.Models
{
    public class GLSModel
    {
        [Display(Name = "Endpoint address")]
        public string EndpointAddress { get; set; }

        [Display(Name = "Amount of suggested shops for customer")]
        public int AmountNearestShops { get; set; }

        [Display(Name = "Shipping price end with (9)")]
        public string PricesEndsWith { get; set; }

        [Display(Name = "Free shipping for order amounts above")]
        public decimal FreeShippingLimit { get; set; }

        public List<AOGLSCountry> GLSCountries { get; set; }

        public bool Tracing { get; set; }

        public string ErrorMessage { get; set; }

        [Display(Name = "Separator between GLS Shop name and address etc.")]
        public string Separator { get; set; }
    }
}
