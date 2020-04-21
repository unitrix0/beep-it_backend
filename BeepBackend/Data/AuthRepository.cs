using BeepBackend.Models;
using BeepBackend.Permissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly BeepDbContext _context;

        public AuthRepository(BeepDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateFirstEnvironment(User newUser)
        {
            var environment = new BeepEnvironment() { Name = $"Zu Hause von {newUser.DisplayName}", User = newUser, DefaultEnvironment = true };
            var permissions = new Permission() { IsOwner = true, User = newUser, Environment = environment, Serial = SerialGenerator.Generate() };

            await _context.AddAsync(environment);
            await _context.AddAsync(permissions);
            await _context.SaveChangesAsync();

            return newUser;
        }


        public async Task<Permission> GetDefaultPermissions(int userId)
        {
            Permission permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Environment.DefaultEnvironment);

            return permission;
        }

        public async Task<Permission> GetUserPermissionForEnvironment(int userId, int environmentId)
        {
            Permission permissions = await _context.Permissions
                .Include(p => p.User)
                .Include(p => p.Environment)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Environment.Id == environmentId);

            return permissions;
        }

        public async Task<IEnumerable<Permission>> GetAllUserPermissions(int userId)
        {
            List<Permission> permissions = await _context.Permissions.Where(p => p.UserId == userId)
                .ToListAsync();

            return permissions;
        }
    }
}