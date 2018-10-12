using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Products
{
    [JsonObject(Title = "attributes_combination")]
    public class ProductAttributeCombinationDto
    {
        [JsonProperty("size_value_name")]
        public string SizeValueName { get; set; }

        [JsonProperty("color_value_name")]
        public string ColorValueName { get; set; }

        [JsonProperty("stock_quantity")]
        public int StockQuantity { get; set; }

        [JsonProperty("allow_out_of_stock")]
        public bool AllowOutOfStockOrders { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("manufacturer_part_number")]
        public string ManufacturerPartNumber { get; set; }

        [JsonProperty("gtin")]
        public string Gtin { get; set; }

        [JsonProperty("overridden_price")]
        public decimal? OverriddenPrice { get; set; }

        [JsonProperty("notify_admin_for_quantity_below")]
        public int NotifyAdminForQuantityBelow { get; set; }
    }
}
