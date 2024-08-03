using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cinema.UI.Models.UserModels
{
    public class ResetPasswordViewModel
    {
        public string UserName { get; set; }

        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [JsonIgnore]
        public string Token { get; set; }

    }
}
