using BeepBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Utrix.WebLib.Authentication;

namespace BeepBackend.Data
{
    public class AuthRepository : AuthRepositoryBase<User>
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public override async Task<User> Register(User user, string password)
        {
            CreatePasswordHash(password, out var hash, out var salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            var environment = new BeepEnvironment() { Name = $"Zu Hause von {user.Username}", User = user };
            var permissions = new Permission() { IsOwner = true, User = user, Environment = environment};


            await _context.AddAsync(user);
            await _context.AddAsync(environment);
            await _context.AddAsync(permissions);
            await _context.SaveChangesAsync();

            return user;
        }

        public override async Task<User> Login(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return null;

            return !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt) ? null : user;
        }

        public override async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}