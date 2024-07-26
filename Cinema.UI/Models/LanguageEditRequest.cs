using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cinema.UI.Models
{
    public class LanguageEditRequest
    {
        [MaxLength(30)]
        public string? Name { get; set; }
        public IFormFile? FlagPhoto { get; set; }
        [JsonIgnore]
        public string? FlagPhotoUrl { get; set; }
    }
}
