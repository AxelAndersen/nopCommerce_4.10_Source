using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Shipping.GLS.Models
{
    public class GLSModel
    {
        [Display(Name = "Endpoint address")]
        public string EndpointAddress { get; set; }

        [Display(Name = "Amount of suggested shops for customer")]
        public int AmountNearestShops { get; set; }

        public string ErrorMessage { get; set; }
    }
}
