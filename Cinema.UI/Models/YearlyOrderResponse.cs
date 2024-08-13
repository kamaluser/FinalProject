using System.Text.Json.Serialization;

namespace Cinema.UI.Models
{
    public class YearlyOrderResponse
    {
        [JsonPropertyName("months")]
        public List<string> Months { get; set; }

        [JsonPropertyName("orders")]
        public List<int> Orders { get; set; }
    }
}
