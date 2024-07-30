namespace Cinema.UI.Models
{
    public class SessionCreateRequest
    {
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public int LanguageId { get; set; }
        public DateTime ShowDateTime { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }
}
