using System.Collections.Generic;
using System.Threading.Tasks;
using BeepBackend.Models;

namespace BeepBackend.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User userToCreate, string password);
        Task<bool> UserExists(string username);
        Task<User> Login(string username, string password);
        Task<IList<string>> GetUserRoles(User userName);
        Task<Permission> GetDefaultPermissions(int userId);
        Task<Permission> GetUserPermissions(int userId, int environmentId);
    }
}