using System.Collections.Generic;
using System.Threading.Tasks;
using BeepBackend.Models;

namespace BeepBackend.Data
{
    public interface IAuthRepository
    {
        Task<User> CreateFirstEnvironment(User userToCreate);
        Task<Permission> GetDefaultPermissions(int userId);
        Task<Permission> GetUserPermissionForEnvironment(int userId, int environmentId);
        Task<IEnumerable<Permission>> GetAllUserPermissions(int userId);
        Task<int> CountDemoUsers();
        Task<User> CreateDemoData(User newUser);
    }
}