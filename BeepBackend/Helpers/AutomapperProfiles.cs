using System.Linq;
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
        }
    }
}
