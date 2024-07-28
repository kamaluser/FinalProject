using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.NewsDtos
{
    public class AdminNewsEditDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class AdminNewsEditDtoValidator : AbstractValidator<AdminNewsEditDto>
    {
        public AdminNewsEditDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(600).WithMessage("Description must not exceed 600 characters.");

            RuleFor(x => x.Image)
                .Must(IsValidContentType).WithMessage("Invalid image format. Only .jpg, .jpeg, and .png are allowed.")
                .Must(IsValidSize).WithMessage("Image size must be less than 5MB.")
                .When(x => x.Image != null);
        }

        private bool IsValidContentType(IFormFile image)
        {
            if (image == null) return false;

            var allowedContentTypes = new[] { "image/jpeg", "image/png" };
            return allowedContentTypes.Contains(image.ContentType);
        }

        private bool IsValidSize(IFormFile image)
        {
            if (image == null) return false;

            return image.Length <= 5 * 1024 * 1024;
        }
    }
}
