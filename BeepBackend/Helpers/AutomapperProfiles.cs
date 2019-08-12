using AutoMapper;
using BeepBackend.DTOs;
using BeepBackend.Models;

namespace BeepBackend.Helpers
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<BeepEnvironment, EnvironmentDto>()
                .ForMember(be => be.Permissions, opt => { opt.MapFrom(src => src.Permissions); });

            CreateMap<Permission, PermissionsDto>()
                .ForMember(p => p.Username, opt => { opt.MapFrom(src => src.User.Username); })
                .ForMember(p => p.UserId, opt => { opt.MapFrom(src => src.User.Id); })
                .ForMember(p => p.EnvironmentId, opt => { opt.MapFrom(src => src.Environment.Id); });

            CreateMap<PermissionsDto, Permission>();

            CreateMap<User, UserForEditDto>();
            CreateMap<User, UserForTokenDto>();
        }
    }
}
