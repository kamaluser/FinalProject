using System.Text.Json.Serialization;

namespace Cinema.UI.Models
{
    public class MovieEditRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TrailerLink { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? AgeLimit { get; set; }
        public IFormFile? Photo { get; set; }
        [JsonIgnore]
        public string? PhotoUrl { get; set; }
        public List<int>? LanguageIds { get; set; }
    }
}
