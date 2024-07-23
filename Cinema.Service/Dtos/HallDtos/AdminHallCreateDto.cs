using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.HallDtos
{
    public class AdminHallCreateDto
    {
        public string Name { get; set; }
        public int SeatCount { get; set; }
        public int BranchId { get; set; }
    }

    public class AdminHallCreateDtoValidator : AbstractValidator<AdminHallCreateDto>
    {
        public AdminHallCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.SeatCount)
                .GreaterThan(0).WithMessage("Seat count must be greater than zero.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Branch ID must be greater than zero.");
        }
    }
}
