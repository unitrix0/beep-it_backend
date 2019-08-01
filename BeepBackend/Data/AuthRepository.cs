using System;
using System.Linq;
using System.Threading.Tasks;
using BeepBackend.Models;
using Microsoft.EntityFrameworkCore;
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
            
            //var environment = new Environment(){ Name = "Zu Hause"};
            //var userEnvironment = new UserEnvironment()
            //{
            //    User = user,
            //    IsOwner = true,
            //    Environment = environment
            //};
            
            //await _context.AddAsync(environment);
            //await _context.AddAsync(userEnvironment);
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public override async Task<User> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}