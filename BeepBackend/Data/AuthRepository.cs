﻿using BeepBackend.Models;
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

        public async Task<int> CountDemoUsers()
        {
            int count = await _context.UserRoles.CountAsync(r => r.Role.Name == RoleNames.Demo);
            return count;
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

        public async Task<User> CreateDemoData(User newUser)
        {
            newUser = await CreateFirstEnvironment(newUser);
            bool demoDataCreated = await _context.CreateDemoDataForUser(newUser.Id);

            return !demoDataCreated ? null : newUser;
        }

        public async Task AddRefreshToken(RefreshToken newRefreshToken)
        {
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenForUser(string refreshTokenString)
        {
            RefreshToken refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenString);
            return refreshToken;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}