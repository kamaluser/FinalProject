using System.Text.Json.Serialization;

namespace Cinema.UI.Models.SessionModels
{
    public class SessionCountResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
