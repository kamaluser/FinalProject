using Newtonsoft.Json;

namespace Cinema.UI.Models
{
    public class OrderRevenueResponse
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
