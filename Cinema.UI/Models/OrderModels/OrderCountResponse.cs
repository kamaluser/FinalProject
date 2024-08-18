using System.Text.Json.Serialization;

namespace Cinema.UI.Models.OrderModels
{
    public class OrderCountResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
