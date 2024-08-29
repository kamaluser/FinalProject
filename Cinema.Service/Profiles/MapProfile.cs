using AutoMapper;
using Cinema.Core.Entites;
using Cinema.Service.Dtos.BranchDtos;
using Cinema.Service.Dtos.HallDtos;
using Cinema.Service.Dtos.LanguageDtos;
using Cinema.Service.Dtos.MovieDtos;
using Cinema.Service.Dtos.NewsDtos;
using Cinema.Service.Dtos.OrderDtos;
using Cinema.Service.Dtos.SessionDtos;
using Cinema.Service.Dtos.SliderDtos;
using Cinema.Service.Dtos.UserDtos;
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

            var request = _accessor.HttpContext?.Request;
            if (request == null)
            {
                throw new InvalidOperationException("HTTP context or request is null.");
            }

            var host = request.Host;
            if (host == null)
            {
                throw new InvalidOperationException("Host is null.");
            }

            var uriBuilder = new UriBuilder(request.Scheme, host.Host, host.Port ?? -1);

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


            //slider(user)
            CreateMap<Slider, UserSliderGetDto>()
           .ForMember(dest => dest.Image, opt => opt.MapFrom((src, dest, destMember, context) =>
           {
               return baseUrl + $"/uploads/sliders/{src.Image}";
           }));


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

            CreateMap<Language, LanguageGetDto>()
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LanguagePhoto, opt => opt.MapFrom(src => $"{baseUrl}/uploads/flags/{src.FlagPhoto}"));


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


            // News (user)
            CreateMap<News, UserNewsGetDto>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    return baseUrl + $"/uploads/news/{src.Image}";
                }));

            // Movie
            CreateMap<Movie, AdminMovieGetDto>()
                .ForMember(dest => dest.LanguageIds, opt => opt.MapFrom(src => src.MovieLanguages.Select(ml => ml.Language.Id).ToList()))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => $"{baseUrl}/uploads/movies/{src.Photo}"));

            CreateMap<AdminMovieCreateDto, Movie>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore());

            CreateMap<AdminMovieEditDto, Movie>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore());

            CreateMap<Movie, UserMovieGetDto>()
                .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.MoviePhoto, opt => opt.MapFrom(src => $"{baseUrl}/uploads/movies/{src.Photo}"))
                .ForMember(dest => dest.Languages, opt => opt.MapFrom(src => src.MovieLanguages.Select(ml => new LanguageGetDto
                {
                    LanguageName = ml.Language.Name,
                    LanguagePhoto = $"{baseUrl}/uploads/flags/{ml.Language.FlagPhoto}"
                }).ToList()))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate))
                .ForMember(dest => dest.AgeLimit, opt => opt.MapFrom(src => src.AgeLimit));

            CreateMap<Session, UserSessionDetailsDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.ShowDateTime, opt => opt.MapFrom(src => src.ShowDateTime))
               .ForMember(dest => dest.HallName, opt => opt.MapFrom(src => src.Hall.Name))
               .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Hall.Branch.Name))
               .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name))
               .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Movie.Title))
               .ForMember(dest => dest.LanguagePhoto, opt => opt.MapFrom((src, dest, destMember, context) =>
               {
                   return $"{baseUrl}/uploads/flags/{src.Language.FlagPhoto}";
               }))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
               .ForMember(dest => dest.TrailerLink, opt => opt.MapFrom(src => src.Movie.TrailerLink));


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

            //AppUser(admin)
            CreateMap<AppUser, AdminGetDto>();
            CreateMap<AppUser, AdminPaginatedGetDto>();


            // Order
            CreateMap<Order, AdminOrderGetDto>()
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
             .ForMember(dest => dest.EmailOfUser, opt => opt.MapFrom(src => src.User.Email))
             .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Session.Movie.Title))
             .ForMember(dest => dest.SessionDate, opt => opt.MapFrom(src => src.Session.ShowDateTime))
             .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Session.Hall.Branch.Name))
             .ForMember(dest => dest.HallName, opt => opt.MapFrom(src => src.Session.Hall.Name))
             .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Session.Language.Name))
             .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
             .ForMember(dest => dest.SeatNumbers, opt => opt.MapFrom(src => src.OrderSeats.Select(os => os.Seat.Number).ToList()));
        }
    }
}