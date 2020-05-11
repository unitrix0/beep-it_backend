using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeepBackend.Models;

namespace BeepBackend.Data
{
    public interface IPermissionsCache
    {
        /// <summary>
        /// Gibt zurück ob der Permission-Serial eines Users mit dem Cache überein stimmt
        /// oder überhaupt vorhanden ist.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="environmentId"></param>
        /// <param name="permissionSerial"></param>
        /// <returns></returns>
        PermissionsChacheResult SerialsMatch(int userId, int environmentId, string permissionSerial);
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
        void Cleanup();
    }
}