using System;
using FluentValidation;

namespace Cinema.Service.Dtos.SessionDtos
{
    public class AdminSessionEditDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public int LanguageId { get; set; }
        public DateTime ShowDateTime { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }

    public class AdminSessionEditDtoValidator : AbstractValidator<AdminSessionEditDto>
    {
        public AdminSessionEditDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.")
                .GreaterThan(0).WithMessage("Id must be a positive integer.");

            RuleFor(x => x.MovieId)
                .NotEmpty().WithMessage("MovieId is required.")
                .GreaterThan(0).WithMessage("MovieId must be a positive integer.");

            RuleFor(x => x.HallId)
                .NotEmpty().WithMessage("HallId is required.")
                .GreaterThan(0).WithMessage("HallId must be a positive integer.");

            RuleFor(x => x.LanguageId)
                .NotEmpty().WithMessage("LanguageId is required.")
                .GreaterThan(0).WithMessage("LanguageId must be a positive integer.");

            RuleFor(x => x.ShowDateTime)
                .NotEmpty().WithMessage("ShowDateTime is required.")
                .GreaterThan(DateTime.Now).WithMessage("ShowDateTime must be in the future.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration must be at least 1 minute.");
        }
    }
}
