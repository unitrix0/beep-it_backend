using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeepBackend.Permissions
{
    public static class PermissionExtensions
    {
        public static string ToBits(this Permission permission)
        {
            var values = new List<short>()
            {
                Convert.ToInt16(permission.ManageUsers),
                Convert.ToInt16(permission.EditArticleSettings),
                Convert.ToInt16(permission.CanScan),
                Convert.ToInt16(permission.IsOwner)
            };

            return string.Join("", values);
        }

        /// <summary>
        /// Prüft ob der übergebene Benutzer die angegeben Berechtigung im angegebenen Environment hat
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="user">User dessen berechtigung geprüft werden soll</param>
        /// <param name="environmentId">Id des Environments in dem der Benutzer berechtigt sein muss.</param>
        /// <param name="requiredPermission">Die benötigte Berechtigung</param>
        /// <returns></returns>
        public static async Task<bool> IsPermitted(this IAuthorizationService authService, ClaimsPrincipal user, int environmentId, PermissionFlags requiredPermission)
        {
            if (environmentId == 0) throw new ArgumentException("Environment ID may not be 0", nameof(environmentId));
            AuthorizationResult result = await authService.AuthorizeAsync(user, null, new[]
            {
                new HasEnvironmentPermissionRequirement(environmentId, requiredPermission),
            });

            return result.Succeeded;
        }

        /// <summary>
        /// Prüft ob der übergebene Benutzer im angegebenen Evnrionment mitglied ist.
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="user">User dessen berechtigung geprüft werden soll</param>
        /// <param name="environmentId">Id des Environments in dem der Benutzer berechtigt sein muss.</param>
        /// <returns></returns>
        public static async Task<bool> IsPermitted(this IAuthorizationService authService, ClaimsPrincipal user, int environmentId)
        {
            if (environmentId == 0) throw new ArgumentException("Environment ID may not be 0", nameof(environmentId));

            AuthorizationResult result = await authService.AuthorizeAsync(user, null, new[]
            {
                new HasEnvironmentPermissionRequirement(environmentId),
            });

            return result.Succeeded;
        }
    }
}
