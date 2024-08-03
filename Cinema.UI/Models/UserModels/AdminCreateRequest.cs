using System.ComponentModel.DataAnnotations;

namespace Cinema.UI.Models.UserModels
{
    public class AdminCreateRequest
    {
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(25)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
