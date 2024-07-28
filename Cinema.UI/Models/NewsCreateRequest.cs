namespace Cinema.UI.Models
{
    public class NewsCreateRequest
    {
        public string Title { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; }
    }
}
