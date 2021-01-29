using AutoMapper;
using BeepBackend.DTOs;
using BeepBackend.Models;

namespace BeepBackend.Articles.DTOs
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Article, ArticleDto>()
                .ForMember(a => a.Stores, opt => opt.MapFrom(src => src.ArticleStores));
            CreateMap<ArticleDto, Article>()
                .ForMember(a => a.ArticleStores, opt => opt.MapFrom(src => src.Stores));

            CreateMap<ArticleUserSetting, ArticleUserSettingDto>();
            CreateMap<ArticleUserSettingDto, ArticleUserSetting>()
                .ForMember(aus => aus.ArticleGroup, opt => opt.Ignore());

            CreateMap<ArticleGroup, ArticleGroupDto>();
            CreateMap<ArticleGroupDto, ArticleGroup>()
                .ForMember(ag => ag.Id, opt => opt.Ignore());

            CreateMap<CheckInDto, StockEntryValue>()
                .ForMember(sev => sev.ExpireDate, opt => opt.MapFrom(src => src.ExpireDate.Date));
            CreateMap<StockEntryValue, CheckInDto>();

            CreateMap<StockEntryValue, StockEntryValueDto>();
            CreateMap<StockEntryValueDto, StockEntryValue>();

            CreateMap<ArticleStore, ArticleStoreDto>();
            CreateMap<ArticleStoreDto, ArticleStore>();
        }
    }
}
