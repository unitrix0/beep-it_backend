using BeepBackend.Data;
using BeepBackend.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Utrix.WebLib;

namespace BeepBackend.Permissions
{
    public class PermissionsChangedMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionsChangedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IPermissionsCache permissionsCache, IAuthRepository authRepo)
        {
            string serial = context.Request.Headers["permissions_serial"];
            int environmentId = Convert.ToInt32(context.Request.Headers["environment_id"]);
            int userId = Convert.ToInt32(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!string.IsNullOrEmpty(serial) && serial.ToLower() != "updating"
                && environmentId > 0 && userId > 0)
            {
                PermissionsChacheResult chacheResult = permissionsCache.SerialsMatch(userId, environmentId, serial);
                switch (chacheResult)
                {
                    case PermissionsChacheResult.DoNotMatch:
                        context.Response.AddCustomHeader("permissions_changed", "true");
                        break;
                    case PermissionsChacheResult.NotCached:
                        IEnumerable<Permission> userPermissions = authRepo.GetAllUserPermissions(userId).Result;
                        permissionsCache.AddEntriesForUser(userId, userPermissions);
                        break;
                }
            }

            await _next(context);
        }
    }
}
