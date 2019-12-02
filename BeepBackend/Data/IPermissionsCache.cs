using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeepBackend.Models;

namespace BeepBackend.Data
{
    public interface IPermissionsCache
    {
        bool SerialsMatch(string userName, int environmentId, string permissionSerial);
        void Update(int userId, int environmentId, Permission permission);
        Permission GetUserPermission(int userId, int environmentId);
        void AddEntriesForUser(int userId, DateTime lifetime, IEnumerable<Permission> permissions);
    }
}