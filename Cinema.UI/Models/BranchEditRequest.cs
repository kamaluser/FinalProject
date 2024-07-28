using System.ComponentModel.DataAnnotations;

namespace Cinema.UI.Models
{
    public class BranchEditRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? Address { get; set; }
    }
}