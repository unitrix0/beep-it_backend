using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeepBackend.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userMgr;
        private readonly SignInManager<User> _signInMgr;

        public AuthRepository(DataContext context, UserManager<User> userMgr, SignInManager<User> signInMgr)
        {
            _context = context;
            _userMgr = userMgr;
            _signInMgr = signInMgr;
        }

        public async Task<User> Register(User newUser, string password)
        {
            IdentityResult result = await _userMgr.CreateAsync(newUser, password);
            if (!result.Succeeded) return null;
            await _userMgr.AddToRoleAsync(newUser, RoleNames.Member);

            var environment = new BeepEnvironment() { Name = $"Zu Hause von {newUser.DisplayName}", User = newUser, DefaultEnvironment = true };
            var permissions = new Permission() { IsOwner = true, User = newUser, Environment = environment, Serial = SerialGenerator.Generate() };

            await _context.AddAsync(environment);
            await _context.AddAsync(permissions);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<User> Login(string username, string password)
        {
            User user = await _userMgr.FindByNameAsync(username);
            if (user == null) return null;
            SignInResult result = await _signInMgr.CheckPasswordSignInAsync(user, password, false);

            return result.Succeeded ? user : null;
        }

        public async Task<IList<string>> GetUserRoles(User user)
        {
            IList<string> roles = await _userMgr.GetRolesAsync(user);
            return roles;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _userMgr.FindByNameAsync(username) != null;
        }

        public async Task<Permission> GetDefaultPermissions(int userId)
        {
            Permission permission = await _context.Permissions
                .Include(p => p.Environment)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Environment.DefaultEnvironment);

            return permission;
        }

        public async Task<Permission> GetUserPermissions(int userId, int environmentId)
        {
            Permission permissions = await _context.Permissions
                .Include(p => p.User)
                .Include(p => p.Environment)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Environment.Id == environmentId);

            return permissions;
        }
    }
}