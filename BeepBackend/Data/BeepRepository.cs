using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Data
{
    public class BeepRepository : IBeepRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userMgr;

        public BeepRepository(DataContext context, UserManager<User> userMgr)
        {
            _context = context;
            _userMgr = userMgr;
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

            return ownerId?.UserId ?? 0;
        }

        public async Task<BeepEnvironment> AddEnvironment(int userId)
        {
            var owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var newEnv = new BeepEnvironment() { Name = "Neue Umgebung", User = owner };
            var permission = new Permission() { IsOwner = true, Environment = newEnv, User = owner, Serial = SerialGenerator.Generate() };

            await _context.Permissions.AddAsync(permission);
            await _context.Environments.AddAsync(newEnv);

            return await _context.SaveChangesAsync() > 0 ? newEnv : null;
        }

        public async Task<IEnumerable<BeepEnvironment>> GetEnvironments(int userId)
        {
            return await _context.Environments
                .Include(e => e.Permissions)
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> DeleteEnvironment(int envId)
        {
            BeepEnvironment environment = await _context.Environments
                .Include(e => e.Permissions)
                .FirstOrDefaultAsync(e => e.Id == envId);

            foreach (Permission permission in environment.Permissions)
            {
                _context.Permissions.Remove(permission);
            }

            _context.Environments.Remove(environment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> InviteMember(string inviteeName, int environmentId)
        {
            User invitee = await _context.Users.FirstOrDefaultAsync(u => u.UserName == inviteeName);

            var invitation = new Invitation()
            {
                Invitee = invitee,
                EnvironmentId = environmentId,
                IssuedAt = DateTime.Now
            };

            await _context.AddAsync(invitation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Invitation> InviteMemberByMail(string email, int envitonmentId)
        {
            bool isNewUser = false;
            User invitee = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (invitee == null)
            {
                isNewUser = true;
                invitee = new User()
                {
                    UserName = email,
                    DisplayName = email,
                    LockoutEnabled = true
                };

                IdentityResult result = await _userMgr.CreateAsync(invitee, SerialGenerator.Generate());
                IdentityResult roleResult = await _userMgr.AddToRoleAsync(invitee, RoleNames.Dummy);
                if (!result.Succeeded || !roleResult.Succeeded) return null;
            }
            
            var invitation = new Invitation()
            {
                Invitee = invitee,
                EnvironmentId = envitonmentId,
                IssuedAt = DateTime.Now,
                Serial = isNewUser ? SerialGenerator.Generate() : string.Empty
            };

            await _context.AddAsync(invitation);
            return await _context.SaveChangesAsync() > 0 ? invitation : null;
        }

        public async Task<int> CountInvitations(int userId)
        {
            return await _context.Invitations.CountAsync(i =>
                i.InviteeId == userId &&
                i.AnsweredOn == DateTime.MinValue);
        }

        public async Task<int> GetInviteeId(int userId, int environmentId)
        {
            Invitation invitation = await _context.Invitations.FirstOrDefaultAsync(i =>
                i.InviteeId == userId && i.EnvironmentId == environmentId);

            return invitation?.InviteeId ?? 0;
        }

        public async Task<bool> JoinEnvironment(int userId, int environmentId)
        {
            var permission = new Permission()
            {
                UserId = userId,
                EnvironmentId = environmentId,
                Serial = SerialGenerator.Generate(),
                CanView = true
            };
            await _context.Permissions.AddAsync(permission);

            Invitation invitation = await _context.Invitations.FirstOrDefaultAsync(i =>
                i.EnvironmentId == environmentId &&
                i.InviteeId == userId);

            invitation.AnsweredOn = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteInvitation(int userId, int environmentId)
        {
            Invitation invitation = await _context.Invitations
                .Include(i => i.Invitee)
                .FirstOrDefaultAsync(i =>
                i.EnvironmentId == environmentId &&
                i.InviteeId == userId);

            if (invitation == null) return false;

            _context.Invitations.Remove(invitation);
            if (await _userMgr.IsInRoleAsync(invitation.Invitee, RoleNames.Dummy))
                _context.Users.Remove(invitation.Invitee);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAnsweredInvitations(int userId)
        {
            List<Invitation> invitations = await _context.Invitations
                .Include(i => i.Environment).ThenInclude(e => e.User)
                .Where(i => i.Environment.User.Id == userId &&
                            i.AnsweredOn != DateTime.MinValue)
                .ToListAsync();

            foreach (Invitation invitation in invitations)
            {
                _context.Invitations.Remove(invitation);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Invitation>> GetReceivedInvitationsForUser(int inviteeId)
        {
            List<Invitation> invitations = await _context.Invitations
                .Include(i => i.Environment).ThenInclude(e => e.User)
                .Where(i => i.InviteeId == inviteeId && i.AnsweredOn == DateTime.MinValue)
                .ToListAsync();

            return invitations;
        }

        public async Task<List<Invitation>> GetSentInvitationsForUserAsync(int userId)
        {
            List<Invitation> invitations = await _context.Invitations
                .Include(i => i.Environment).ThenInclude(e => e.User)
                .Include(i => i.Invitee)
                .Where(i => i.Environment.UserId == userId)
                .OrderByDescending(i => i.IssuedAt)
                .ToListAsync();

            return invitations;
        }

        public async Task<int> GetInviterId(int environmentId, int inviteeId)
        {
            Invitation invitation = await _context.Invitations
                .Include(i => i.Environment)
                .FirstOrDefaultAsync(i => i.EnvironmentId == environmentId &&
                                          i.InviteeId == inviteeId);

            return invitation?.Environment.UserId ?? 0;
        }
    }
}