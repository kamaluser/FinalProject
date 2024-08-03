using System.Text.Json.Serialization;

namespace Cinema.UI.Models.SettingModels
{
    public class SettingEditRequest
    {
        public IFormFile? Logo { get; set; }
        [JsonIgnore]
        public string? LogoUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FacebookUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TelegramUrl { get; set; }
        public string? ContactAddress { get; set; }
        public string? ContactEmailAddress { get; set; }
        public string? ContactWorkHours { get; set; }
        public string? ContactMarketingDepartment { get; set; }
        public string? ContactMap { get; set; }
        public string? AboutTitle { get; set; }
        public string? AboutDesc { get; set; }
    }
}
