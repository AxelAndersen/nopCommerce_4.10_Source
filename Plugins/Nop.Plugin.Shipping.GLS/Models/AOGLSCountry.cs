using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Shipping.GLS.Models
{
    public class AOGLSCountry
    {
        [Key]
        public string ThreeLetterISOCode { get; set; }
        public string TwoLetterGLSCode { get; set; }
        public string GLSNumber { get; set; }
        public string CountryName { get; set; }
        public bool SupportParcelShop { get; set; }
        public decimal ShippingPrice_0_1 { get; set; }
        public decimal ShippingPrice_1_5 { get; set; }
        public decimal ShippingPrice_5_10 { get; set; }
        public decimal ShippingPrice_10_15 { get; set; }
        public decimal ShippingPrice_15_20 { get; set; }
        public decimal ShippingPrice_20_25 { get; set; }
        public decimal ShippingPrice_25_30 { get; set; }
    }
}