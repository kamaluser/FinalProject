using System.ComponentModel.DataAnnotations;

namespace Cinema.UI.Models.UserModels
{
    public class AdminProfileEditRequest
    {
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string UserName { get; set; }

        [MaxLength(25)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        
        [MaxLength(25)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        
        [MaxLength(25)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; }
    }
}