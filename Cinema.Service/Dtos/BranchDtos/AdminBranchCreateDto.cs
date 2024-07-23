using Cinema.Core.Entites;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.BranchDtos
{
    public class AdminBranchCreateDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class AdminBranchCreateDtoValidator : AbstractValidator<AdminBranchCreateDto>
    {
        public AdminBranchCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");
        }
    }
}
