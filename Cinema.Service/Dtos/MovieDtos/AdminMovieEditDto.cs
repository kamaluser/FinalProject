﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Dtos.MovieDtos
{
    public class AdminMovieEditDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TrailerLink { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? AgeLimit { get; set; }
        public IFormFile? Photo { get; set; }
        public List<int>? LanguageIds { get; set; }
    }

    public class AdminMovieEditDtoValidator : AbstractValidator<AdminMovieEditDto>
    {
        public AdminMovieEditDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.TrailerLink)
                .Must(IsValidUrl).WithMessage("Invalid URL format.");

            RuleFor(x => x.AgeLimit)
                .MaximumLength(30).WithMessage("AgeLimit must not exceed 30 characters.");

            When(x => x.Photo != null, () =>
            {
                RuleFor(x => x.Photo)
                    .Must(IsValidContentType).WithMessage("Invalid photo format. Only .jpg, .jpeg, and .png are allowed.")
                    .Must(IsValidSize).WithMessage("Photo size must be less than 5MB.");
            });

            RuleFor(x => x.LanguageIds)
                .Must(ids => ids.All(id => id > 0)).WithMessage("Language IDs must be greater than zero.");
        }

        private bool IsValidUrl(string url)
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
