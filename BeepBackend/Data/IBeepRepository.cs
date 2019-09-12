using System.Collections.Generic;
using BeepBackend.Models;
using System.Threading.Tasks;

namespace BeepBackend.Data
{
    public interface IBeepRepository
    {
        Task<User> GetUser(int id);
        Task<Permission> GetUserPermission(int environmentId, int userId);
        Task<bool> SaveAll();
        Task<int> GetEnvironmentOwnerId(int environmentId);
        Task<BeepEnvironment> AddEnvironment(int userId);
        Task<IEnumerable<BeepEnvironment>> GetEnvironments(int userId);
    }
}
