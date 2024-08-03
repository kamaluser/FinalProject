using Cinema.Core.Entites;
using Cinema.UI.Models.LanguageModels;

namespace Cinema.UI.Models.MovieModels
{
    public class MovieGetResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string TrailerLink { get; set; }

        public string AgeLimit { get; set; }
        public string Photo { get; set; }
        public List<LanguageGetResponse>? Languages { get; set; }
        public List<int>? LanguageIds { get; set; }
    }
}
