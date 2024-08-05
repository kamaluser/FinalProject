using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.UserDtos.MemberDtos
{
    public class MemberRegisterDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class MemberRegisterDtoValidator : AbstractValidator<MemberRegisterDto>
    {
        public MemberRegisterDtoValidator()
        {

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .Length(6, 25).WithMessage("UserName must be between 6 and 25 characters.");


            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");


            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(50).WithMessage("FullName must be less than 50 characters.");


            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.");


            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("ConfirmPassword is required.")
                .Equal(x => x.Password).WithMessage("Password and Confirmation Password do not match.");
        }
    }
}
