using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;

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
        /// Prüft ob der übergebene Benutzer für einen API Call berechtigt ist
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="user">User dessen berechtigung geprüft werden soll</param>
        /// <param name="environmentId">Id des Environments in dem der Benutzer berechtigt sein muss. Falls 0 wird die ID aus den User-Claims verwendet</param>
        /// <param name="requiredPermission">Die benötigte Berechtigung</param>
        /// <returns></returns>
        public static async Task<bool> IsPermitted(this IAuthorizationService authService, ClaimsPrincipal user, int environmentId, PermissionFlags requiredPermission)
        {
            environmentId = environmentId == 0
                ? int.Parse(user.FindFirst(c => c.Type == BeepClaimTypes.EnvironmentId).Value)
                : environmentId;
            AuthorizationResult result = await authService.AuthorizeAsync(user, null, new[]
            {
                new HasEnvironmentPermissionRequirement(environmentId, requiredPermission),
            });

            return result.Succeeded;
        }
    }
}
