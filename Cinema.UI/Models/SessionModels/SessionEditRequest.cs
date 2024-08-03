namespace Cinema.UI.Models.SessionModels
{
    public class SessionEditRequest
    {
        public int? MovieId { get; set; }
        public int? HallId { get; set; }
        public int? LanguageId { get; set; }
        public DateTime? ShowDateTime { get; set; }
        public decimal? Price { get; set; }
        public int? Duration { get; set; }
    }
}
