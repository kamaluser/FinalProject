using System.ComponentModel.DataAnnotations;

namespace Cinema.UI.Models.HallModels
{
    public class HallEditRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        public int? SeatCount { get; set; }
        public int? BranchId { get; set; }
    }
}
