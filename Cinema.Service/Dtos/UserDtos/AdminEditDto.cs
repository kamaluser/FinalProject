using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Service.Dtos.UserDtos
{
    public class AdminEditDto
    {
        public string UserName { get; set; }
        [MinLength(8)]
        [MaxLength(25)]
        public string? CurrentPassword { get; set; }
        [MinLength(8)]
        [MaxLength(25)]
        public string? NewPassword { get; set; }
        [MinLength(8)]
        [MaxLength(25)]
        public string? ConfirmPassword { get; set; }
    }

    public class AdminEditDtoValidator : AbstractValidator<AdminEditDto>
    {
        public AdminEditDtoValidator()
        {
            RuleFor(x => x.UserName)
                .MaximumLength(25)
                .MinimumLength(2);

            RuleFor(x => x.CurrentPassword)
                .MaximumLength(25)
                .MinimumLength(8);

            RuleFor(x => x.NewPassword)
                .MaximumLength(25)
                .MinimumLength(8);

            RuleFor(x => x.ConfirmPassword)
                .MaximumLength(25)
                .MinimumLength(8);

            RuleFor(x => x)
               .Must(x => x.NewPassword == x.ConfirmPassword)
               .WithMessage("New password and confirm password must be the same.");
        }
    }
}