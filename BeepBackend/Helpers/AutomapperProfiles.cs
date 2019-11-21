using AutoMapper;
using BeepBackend.DTOs;
using BeepBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeepBackend.Helpers
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<BeepEnvironment, EnvironmentDto>();

            CreateMap<Permission, PermissionsDto>()
                .ForMember(p => p.Username, opt => { opt.MapFrom(src => src.User.UserName); });

            CreateMap<PermissionsDto, Permission>();

            CreateMap<User, UserForEditDto>()
                .ForMember(u => u.Environments, opt =>
                {
                    opt.MapFrom(delegate(User user, UserForEditDto dto)
                    {
                        List<BeepEnvironment> envs = user.Environments.ToList();
                        envs.AddRange(user.Permissions
                            .Where(p => p.ManageUsers)
                            .Select(p => p.Environment));
                        return envs;
                    });
                });

            CreateMap<User, UserForTokenDto>();

            CreateMap<Invitation, InvitationListItemDto>()
                .ForMember(i => i.EnvironmentName, opt => { opt.MapFrom(src => src.Environment.Name); })
                .ForMember(i => i.EnvironmentId, opt => { opt.MapFrom(src => src.Environment.Id); })
                .ForMember(i => i.Inviter, opt => opt.MapFrom(src => src.Environment.User.DisplayName))
                .ForMember(i => i.Invitee, opt => opt.MapFrom(src => src.Invitee.DisplayName))
                .ForMember(i => i.IsAnswered, opt => opt.MapFrom(src => src.AnsweredOn != DateTime.MinValue));
        }
    }
}
