﻿using System.Collections.Generic;
using BeepBackend.Models;
using System.Threading.Tasks;

namespace BeepBackend.Data
{
    public interface IBeepRepository
    {
        Task<User> GetUser(int id);
        Task<Permission> GetUserPermission(int environmentId, int userId);
        Task<bool> SaveAll();
        Task<int> GetEnvironmentOwnerId(int environmentId);
        Task<BeepEnvironment> AddEnvironment(int userId);
        Task<IEnumerable<BeepEnvironment>> GetEnvironments(int userId);
        Task<bool> DeleteEnvironment(int envId);
        Task<bool> InviteMember(string inviteeName, int environmentId);
        Task<int> CountInvitations(int userId);
        Task<int> GetInviteeId(int userId, int environmentId);
        Task<bool> JoinEnvironment(int userId, int environmentId);
        Task<List<Invitation>> GetReceivedInvitationsForUser(int userId);
        Task<List<Invitation>> GetSentInvitationsForUserAsync(int userId);
        Task<bool> DeleteInvitation(int userId, int environmentId);
        Task<bool> DeleteAnsweredInvitations(int userId);
        Task<int> GetInviterId(int environmentId, int inviteeId);
        Task<Invitation> InviteMemberByMail(string email, int envitonmentId);
    }
}
