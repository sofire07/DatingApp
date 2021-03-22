using AutoMapper;
using Model.DataTransfer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Model.Helpers
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                     src.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<ApplicationUser, UserLoggedInDto>()
                 .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                     src.Photos.FirstOrDefault(x => x.IsMain).Url));


            CreateMap<Photo, PhotoDto>();
            CreateMap<PhotoDto, Photo>();
            CreateMap<UserLoggedInDto, ApplicationUser>();
            CreateMap<UserDto, ApplicationUser>();
            CreateMap<UserDto, UserLoggedInDto>();
        }
    }
}
