using AutoMapper;
using MySuparApp.Models.Authentication;

namespace MySuparApp.Shared
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {

        CreateMap<UserModel, EntityUserModel>();

            CreateMap<NewUserModel, EntityUserModel>()
                   .ForMember(dest => dest.UserId, opt =>
                       opt.Condition(src => !string.IsNullOrEmpty(src.UserId)));

            CreateMap<NewUserModel, UserModel>();

            CreateMap<EntityUserModel, UserModel>();

        }
    }
}
