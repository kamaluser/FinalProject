namespace Cinema.UI.Models
{
    public class SessionListItemGetResponse
    {
        public int Id { get; set; }
        public string MovieTitle { get; set; }
        public string HallName { get; set; }
        public DateTime ShowDateTime { get; set; }
        public decimal Price { get; set; }
    }
}
