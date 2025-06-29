using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.CourseDtos;
using Application.Dtos.LessonAccessCodeDtos;
using Application.Dtos.LessonDtos;
using Application.Dtos.SubscriptionDtos;
using Application.Dtos.UserDtos;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.EducationalLevel, opt => opt.MapFrom(src => src.EducationalLevel.ToString()));
            CreateMap<CourseDto, Course>()
                .ForMember(dest => dest.EducationalLevel, opt => opt.MapFrom(src => Enum.Parse<Domain.Enums.EducationalLevel>(src.EducationalLevel)));

            CreateMap<Lesson, LessonDto>();
            CreateMap<LessonDto, Lesson>();

            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade.ToString()));
            CreateMap<SubscriptionDto, Subscription>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<SubscriptionType>(src.Type)))
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => Enum.Parse<EducationalLevel>(src.Grade)));

            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade.ToString()));
            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => Enum.Parse<EducationalLevel>(src.Grade)));

            CreateMap<LessonAccessCode, LessonAccessCodeDto>();
            CreateMap<LessonAccessCodeDto, LessonAccessCode>();
        }
    }
}
