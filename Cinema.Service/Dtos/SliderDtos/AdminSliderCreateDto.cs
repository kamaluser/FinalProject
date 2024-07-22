using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.SliderDtos
{
    public class AdminSliderCreateDto
    {
        public int Order { get; set; }
        public IFormFile Image { get; set; }
    }

    public class AdminSliderCreateDtoValidator : AbstractValidator<AdminSliderCreateDto>
    {
        public AdminSliderCreateDtoValidator()
        {
            RuleFor(dto => dto.Order).NotEmpty().GreaterThan(0).WithMessage("Order must be greater than zero.");

            RuleFor(x => x).Custom((dto, context) =>
            {
                if (dto.Image != null)
                {
                    if (dto.Image.Length > 5 * 1024 * 1024)
                    {
                        context.AddFailure("ImageFile", "File must be less or equal than 5MB.");
                    }

                    var allowedContentTypes = new[] { "image/jpeg", "image/png" };
                    if (!allowedContentTypes.Contains(dto.Image.ContentType))
                    {
                        context.AddFailure("ImageFile", "Invalid photo format. Only .jpg, .jpeg, and .png are allowed.");
                    }
                }
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
