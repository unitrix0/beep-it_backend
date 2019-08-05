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

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Environments)
                .ThenInclude(e => e.EnvironmentPermissions)
                .ThenInclude(ep => ep.Permission)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }
    }
}