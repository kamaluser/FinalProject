namespace Cinema.UI.Models.SessionModels
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
