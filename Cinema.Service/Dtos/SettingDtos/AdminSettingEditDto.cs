using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.SettingDtos
{
    public class AdminSettingEditDto
    {
        public IFormFile? Logo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FacebookUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TelegramUrl { get; set; }
        public string? ContactAddress { get; set; }
        public string? ContactEmailAddress { get; set; }
        public string? ContactWorkHours { get; set; }
        public string? ContactMarketingDepartment { get; set; }
        public string? ContactMap { get; set; }
        public string? AboutTitle { get; set; }
        public string? AboutDesc { get; set; }
    }

    public class AdminSettingEditDtoValidator : AbstractValidator<AdminSettingEditDto>
    {
        public AdminSettingEditDtoValidator()
        {
            When(x => x.Logo != null, () =>
            {
                RuleFor(x => x.Logo)
                    .Must(IsValidContentType).WithMessage("Invalid photo format. Only .jpg, .jpeg, and .png are allowed.")
                    .Must(IsValidSize).WithMessage("Photo size must be less than 5MB.");
            });

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?\d{10,15}$")
                .WithMessage("Phone number format is not valid.");

            RuleFor(x => x.FacebookUrl)
                .Must(BeAValidUrl)
                .WithMessage("Facebook URL format is not valid.");

            RuleFor(x => x.YoutubeUrl)
                .Must(BeAValidUrl)
                .WithMessage("Youtube URL format is not valid.");

            RuleFor(x => x.InstagramUrl)
                .Must(BeAValidUrl)
                .WithMessage("Instagram URL format is not valid.");

            RuleFor(x => x.TelegramUrl)
                .Must(BeAValidUrl)
                .WithMessage("Telegram URL format is not valid.");

            RuleFor(x => x.ContactEmailAddress)
                .EmailAddress()
                .WithMessage("Contact email address format is not valid.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
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
