using System.ComponentModel.DataAnnotations;

namespace Cinema.UI.Models.LanguageModels
{
    public class LanguageCreateRequest
    {
        [MaxLength(30)]
        public string Name { get; set; }
        public IFormFile FlagPhoto { get; set; }

    }
}
