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
            CreateMap<BeepEnvironment, EnvironmentDto>()
                .ForMember(e => e.OwnerId,
                    opt => opt.MapFrom(src => src.Permissions.FirstOrDefault(p => p.IsOwner).UserId));

            CreateMap<Permission, PermissionsDto>()
                .ForMember(p => p.Username, opt => { opt.MapFrom(src => src.User.UserName); });

            CreateMap<PermissionsDto, Permission>();

            CreateMap<User, UserForEditDto>()
                .ForMember(u => u.Environments, opt =>
                {
                    opt.MapFrom(delegate (User user, UserForEditDto dto)
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

            CreateMap<Article, ArticleDto>()
                .ForMember(a => a.GroupId, opt => opt.MapFrom(src => src.ArticleGroupId))
                .ForMember(a => a.Stores, opt => opt.MapFrom(src => src.ArticleStores));
            CreateMap<ArticleDto, Article>()
                .ForMember(a => a.ArticleGroupId, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(a => a.ArticleStores, opt => opt.MapFrom(src => src.Stores));

            CreateMap<ArticleUserSetting, ArticleUserSettingDto>();
            CreateMap<ArticleUserSettingDto, ArticleUserSetting>();

            CreateMap<CheckInDto, StockEntryValue>()
                .ForMember(sev => sev.ExpireDate, opt => opt.MapFrom(src => src.ExpireDate.Date));
            CreateMap<StockEntryValue, CheckInDto>();

            CreateMap<StockEntryValue, StockEntryValueDto>();
            CreateMap<StockEntryValueDto, StockEntryValue>();

            CreateMap<ArticleStore, ArticleStoreDto>();
            CreateMap<ArticleStoreDto, ArticleStore>();

            CreateMap<Camera, CameraDto>();
            CreateMap<CameraDto, Camera>();

            CreateMap<ActivityLogEntry, ActivityLogEntryDto>();
            CreateMap<ActivityLogEntryDto, ActivityLogEntry>();

            CreateMap<ShoppingListEntry, ShoppingListEntryDto>();
            CreateMap<ShoppingListArticleEntry, ShoppingListArticleEntryDto>();
        }
    }

    
}
