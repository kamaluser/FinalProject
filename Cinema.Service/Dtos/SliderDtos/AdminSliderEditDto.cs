using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.SliderDtos
{
    public class AdminSliderEditDto
    {
        public int? Order { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class AdminSliderEditDtoValidator : AbstractValidator<AdminSliderEditDto>
    {
        public AdminSliderEditDtoValidator()
        {
            RuleFor(dto => dto.Order).GreaterThan(0).WithMessage("Order must be greater than zero.");

            When(dto => dto.Image != null, () =>
            {
                RuleFor(x => x.Image)
                    .Must(IsValidContentType).WithMessage("Invalid photo format. Only .jpg, .jpeg, and .png are allowed.")
                    .Must(IsValidSize).WithMessage("Photo size must be less than 5MB.");
            });
        }
        private bool IsValidContentType(IFormFile photo)
        {

          /*  if (photo == null)
                return false;*/

            var allowedContentTypes = new[] { "image/jpeg", "image/png" };
            return allowedContentTypes.Contains(photo.ContentType);
        }

        private bool IsValidSize(IFormFile photo)
        {

            /*if (photo == null)
                return false;*/

            return photo.Length <= 5 * 1024 * 1024;
        }
    }
}
