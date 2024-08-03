namespace Cinema.UI.Models.MovieModels
{
    public class MovieCreateRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TrailerLink { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string AgeLimit { get; set; }
        public IFormFile Photo { get; set; }
        public List<int> LanguageIds { get; set; }
    }
}
