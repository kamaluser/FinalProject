using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Service.Dtos.BranchDtos;
using Cinema.Service.Dtos.HallDtos;
using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Service.Dtos.MovieDtos;
using Cinema.Service.Dtos.NewsDtos;
using Cinema.Service.Dtos.SessionDtos;
using Cinema.Service.Dtos.SliderDtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Profiles
{
    public class MapProfile:Profile
    {
        private readonly IHttpContextAccessor _accessor;

        public MapProfile(IHttpContextAccessor accessor)
        {
            _accessor = accessor;

            var uriBuilder = new UriBuilder(_accessor.HttpContext.Request.Scheme, _accessor.HttpContext.Request.Host.Host, _accessor.HttpContext.Request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }
            string baseUrl = uriBuilder.Uri.AbsoluteUri;

            //slider(admin)
            CreateMap<Slider, AdminSliderGetDto>()
             .ForMember(dest => dest.Image, opt => opt.MapFrom((src, dest, destMember, context) =>
             {
                 var baseUrl = _accessor.HttpContext.Request.Scheme + "://" + _accessor.HttpContext.Request.Host.Value;
                 return baseUrl + $"/uploads/sliders/{src.Image}";
             }));

            CreateMap<AdminSliderCreateDto, Slider>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            CreateMap<AdminSliderEditDto, Slider>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            //branch(admin)

            CreateMap<Branch, AdminBranchGetDto>();
            CreateMap<AdminBranchCreateDto, Branch>();
            CreateMap<AdminBranchEditDto, Branch>();

            //hall(admin)
            CreateMap<Hall, AdminHallGetDto>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name));

            CreateMap<AdminHallCreateDto, Hall>();
            CreateMap<AdminHallEditDto, Hall>();

            // language(admin)
            CreateMap<Language, AdminLanguageGetDto>()
                .ForMember(dest => dest.FlagPhoto, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    return baseUrl + $"/uploads/flags/{src.FlagPhoto}";
                }));

            CreateMap<AdminLanguageCreateDto, Language>()
                .ForMember(dest => dest.FlagPhoto, opt => opt.Ignore());

            CreateMap<AdminLanguageEditDto, Language>()
                .ForMember(dest => dest.FlagPhoto, opt => opt.Ignore());

            // news(admin)
            CreateMap<News, AdminNewsGetDto>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    return baseUrl + $"/uploads/news/{src.Image}";
                }));

            CreateMap<AdminNewsCreateDto, News>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            CreateMap<AdminNewsEditDto, News>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            // Movie
            CreateMap<Movie, AdminMovieGetDto>()
                .ForMember(dest => dest.LanguageIds, opt => opt.MapFrom(src => src.MovieLanguages.Select(ml => ml.Language.Id).ToList()))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => $"{baseUrl}/uploads/movies/{src.Photo}"));

            CreateMap<AdminMovieCreateDto, Movie>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore());

            CreateMap<AdminMovieEditDto, Movie>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore());

            // Session(admin)
            CreateMap<Session, AdminSessionGetDto>()
           .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
           .ForMember(dest => dest.HallName, opt => opt.MapFrom(src => src.Hall.Name))
           .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Hall.Branch.Name))
           .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name));

            CreateMap<AdminSessionCreateDto, Session>()
                .ForMember(dest => dest.Movie, opt => opt.Ignore())
                .ForMember(dest => dest.Hall, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore());

            CreateMap<AdminSessionEditDto, Session>()
                .ForMember(dest => dest.Movie, opt => opt.Ignore())
                .ForMember(dest => dest.Hall, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore());
        }
    }
}