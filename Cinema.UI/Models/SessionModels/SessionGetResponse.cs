using Cinema.Core.Entites;

namespace Cinema.UI.Models.SessionModels
{
    public class SessionGetResponse
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public int LanguageId { get; set; }
        public string MovieTitle { get; set; }
        public string HallName { get; set; }
        public string LanguageName { get; set; }
        public DateTime ShowDateTime { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }
}
