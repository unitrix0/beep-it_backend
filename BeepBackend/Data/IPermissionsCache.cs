using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeepBackend.Models;

namespace BeepBackend.Data
{
    public interface IPermissionsCache
    {
        bool SerialsMatch(int userName, int environmentId, string permissionSerial);
        void Update(int userId, int environmentId, Permission permission);
        /// <summary>
        /// Gibt das <see cref="Permission"/> Objekt für den angegebenen Benutzer im angegebenen
        /// Environment zurück. Falls nichts gefunden wird, null.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="environmentId"></param>
        /// <returns></returns>
        Permission GetUserPermission(int userId, int environmentId);
        void AddEntriesForUser(int userId, IEnumerable<Permission> permissions);
    }
}