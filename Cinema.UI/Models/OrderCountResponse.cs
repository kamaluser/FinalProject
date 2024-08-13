using System.Text.Json.Serialization;

namespace Cinema.UI.Models
{
    public class OrderCountResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
