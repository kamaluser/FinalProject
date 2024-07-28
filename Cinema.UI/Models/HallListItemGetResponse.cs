using System.Security.Permissions;

namespace Cinema.UI.Models
{
    public class HallListItemGetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SeatCount { get; set; }
        public string BranchName { get; set; }
    }
}
