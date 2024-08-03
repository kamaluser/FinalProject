using FluentValidation;

namespace Cinema.Service.Dtos.UserDtos
{
    public class AdminEditDto
    {
        public string UserName { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
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