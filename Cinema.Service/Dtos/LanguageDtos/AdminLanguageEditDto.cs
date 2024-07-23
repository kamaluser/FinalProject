using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.LanguageDtos
{
    public class AdminLanguageEditDto
    {
        public string? Name { get; set; }
        public IFormFile? FlagPhoto { get; set; }
    }

    public class AdminLanguageEditDtoValidator : AbstractValidator<AdminLanguageEditDto>
    {
        public AdminLanguageEditDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(30).WithMessage("Name must not exceed 100 characters.");

            When(x => x.FlagPhoto != null, () =>
            {
                RuleFor(x => x.FlagPhoto)
                    .Must(IsValidContentType).WithMessage("Invalid photo format. Only .jpg, .jpeg, and .png are allowed.")
                    .Must(IsValidSize).WithMessage("Photo size must be less than 5MB.");
            });
        }

        private bool IsValidContentType(IFormFile photo)
        {

            if (photo == null)
                return false;

            var allowedContentTypes = new[] { "image/jpeg", "image/png" };
            return allowedContentTypes.Contains(photo.ContentType);
        }

        private bool IsValidSize(IFormFile photo)
        {

            if (photo == null)
                return false;

            return photo.Length <= 5 * 1024 * 1024;
        }
    }
}
