using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Cinema.UI.Models
{
    public class NewsEditRequest
    {
        [MaxLength(200)]
        public string? Title { get; set; }
        [MaxLength(600)]
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        [JsonIgnore]
        public string? ImageUrl { get; set; }
    }
}
