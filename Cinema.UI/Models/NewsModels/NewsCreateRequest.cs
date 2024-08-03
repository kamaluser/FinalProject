namespace Cinema.UI.Models.NewsModels
{
    public class NewsCreateRequest
    {
        public string Title { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; }
    }
}
