using BeepBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BeepBackend.Helpers;
using Microsoft.AspNetCore.Identity;
using Utrix.WebLib.Authentication;

namespace BeepBackend.Data
{
    public class AuthRepository : AuthRepositoryBase<User>
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

        public override async Task<User> Register(User newUser, string password)
        {
            IdentityResult result = await _userMgr.CreateAsync(newUser, password);
            if (!result.Succeeded) return null;
            await _userMgr.AddToRoleAsync(newUser, RoleNames.Member);

            var environment = new BeepEnvironment() { Name = $"Zu Hause von {newUser.DisplayName}", User = newUser };
            var permissions = new Permission() { IsOwner = true, User = newUser, Environment = environment};

            await _context.AddAsync(environment);
            await _context.AddAsync(permissions);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public override async Task<User> Login(string username, string password)
        {
            User user = await _userMgr.FindByNameAsync(username);
            if (user == null) return null;
            SignInResult result = await _signInMgr.CheckPasswordSignInAsync(user, password, false);

            return result.Succeeded ? user : null;
        }

        public override async Task<bool> UserExists(string username)
        {
            User user = await _userMgr.FindByNameAsync(username);
            return user != null;
        }
    }
}