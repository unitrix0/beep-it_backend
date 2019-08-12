using BeepBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BeepBackend.Data
{
    public class BeepRepository : IBeepRepository
    {
        private readonly DataContext _context;

        public BeepRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Environments)
                .ThenInclude(e => e.Permissions)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<Permission> GetUserPermission(int environmentId, int userId)
        {
            var permission = await _context.Permissions
                .Include(p => p.Environment)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.User.Id == userId && p.Environment.Id == environmentId);

            return permission;
        }

        public async Task<int> GetEnvironmentOwnerId(int environmentId)
        {
            var ownerId = await _context.Environments.FirstOrDefaultAsync(e => e.Id == environmentId);

            return ownerId?.Id ?? 0;
        }
    }

}