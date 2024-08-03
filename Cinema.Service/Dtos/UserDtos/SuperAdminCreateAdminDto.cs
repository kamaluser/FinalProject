using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.UserDtos
{
    public class SuperAdminCreateAdminDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class SuperAdminCreateDtoValidator : AbstractValidator<SuperAdminCreateAdminDto>
    {
        public SuperAdminCreateDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(25).MinimumLength(2);

            RuleFor(x => x.Password).NotEmpty().MaximumLength(25).MinimumLength(2);
        }
    }
}
