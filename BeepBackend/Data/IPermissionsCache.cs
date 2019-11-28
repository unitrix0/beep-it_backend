using System;
using BeepBackend.Models;

namespace BeepBackend.Data
{
    public interface IPermissionsCache
    {
        bool SerialsMatch(string userName, int environmentId, string permissionSerial);
        void AddEntry(int userName, int environmentId, Permission permission, DateTime lifetime);
        void Update(int userId, int environmentId, Permission permission);
        Permission GetUserPermission(int userId, int environmentId);
    }
}