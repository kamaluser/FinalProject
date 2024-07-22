using AutoMapper;
using Cinema.Core.Entites;
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
        }
    }
}