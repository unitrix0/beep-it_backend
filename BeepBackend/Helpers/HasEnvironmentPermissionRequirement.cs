using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;

namespace BeepBackend.Helpers
{
    public class HasEnvironmentPermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Id des <see cref="BeepEnvironment"/> in dem der Benutzer berechtigt sein muss
        /// </summary>
        public int EnvironmentId { get; set; }
        /// <summary>
        /// Berechtigung(en) die der Benutzer benötigt
        /// </summary>
        public PermissionFlags Permission { get; set; }

        public HasEnvironmentPermissionRequirement(int environmentId, PermissionFlags permission)
        {
            EnvironmentId = environmentId;
            Permission = permission;
        }
    }
}