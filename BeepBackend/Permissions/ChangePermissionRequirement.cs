using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;

namespace BeepBackend.Permissions
{
    public class ChangePermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Id des <see cref="BeepEnvironment"/> in dem der Benutzer berechtigt sein muss
        /// </summary>
        public int EnvironmentId { get; }
        /// <summary>
        /// Id des Besitzers des <see cref="BeepEnvironment"/>s
        /// </summary>
        public int EnvironmentOwnerId { get; }
        /// <summary>
        /// Id des Benutzer dessen Berechtigung angepasst werden soll
        /// </summary>
        public int PermissionsUserId { get; }
        /// <summary>
        /// Berechtigung(en) die der Benutzer benötigt
        /// </summary>
        public PermissionFlags Permission { get; }

        public ChangePermissionRequirement(int environmentId, int environmentOwnerId, int permissionsUserId, PermissionFlags permission)
        {
            EnvironmentId = environmentId;
            EnvironmentOwnerId = environmentOwnerId;
            PermissionsUserId = permissionsUserId;
            Permission = permission;
        }
    }
}