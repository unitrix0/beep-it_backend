using AutoMapper;
using BeepBackend.DTOs;
using BeepBackend.Models;
using System.Linq;

namespace BeepBackend.Helpers
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<BeepEnvironment, EnvironmentDto>()
                .ForMember(be => be.Permissions, 
                    opt => { opt.MapFrom(src => src.Permissions
                        .OrderByDescending(p => p.IsOwner)
                        .ThenBy(p => p.User.UserName)); });

            CreateMap<Permission, PermissionsDto>()
                .ForMember(p => p.Username, opt => { opt.MapFrom(src => src.User.UserName); })
                .ForMember(p => p.UserId, opt => { opt.MapFrom(src => src.User.Id); })
                .ForMember(p => p.EnvironmentId, opt => { opt.MapFrom(src => src.Environment.Id); });

            CreateMap<PermissionsDto, Permission>();

            CreateMap<User, UserForEditDto>();
            CreateMap<User, UserForTokenDto>();

            CreateMap<Invitation, InvitationListItemDto>()
                .ForMember(i => i.EnvironmentName, opt => { opt.MapFrom(src => src.Environment.Name); })
                .ForMember(i => i.EnvironmentId, opt => { opt.MapFrom(src => src.Environment.Id); })
                .ForMember(i => i.Inviter, opt => opt.MapFrom(src => src.Environment.User.DisplayName));
        }
    }
}
