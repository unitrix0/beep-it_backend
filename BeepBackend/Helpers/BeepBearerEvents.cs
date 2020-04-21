using BeepBackend.Data;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Utrix.WebLib;

namespace BeepBackend.Helpers
{
    public class BeepBearerEvents : JwtBearerEvents
    {
        private readonly IPermissionsCache _cache;
        private readonly IAuthRepository _authRepo;

        public BeepBearerEvents(IPermissionsCache cache, IAuthRepository authRepo)
        {
            _cache = cache;
            _authRepo = authRepo;
        }

        public override Task TokenValidated(TokenValidatedContext context)
        {
            string serial = context.Request.Headers["PermissionsSerial"];
            int environmentId = Convert.ToInt32(context.Request.Headers["EnvironmentId"]);
            int userId = Convert.ToInt32(context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value);

            PermissionsChacheResult chacheResult = _cache.SerialsMatch(userId, environmentId, serial);

            if (chacheResult == PermissionsChacheResult.NotCached)
            {
                IEnumerable<Permission> userPermissions = _authRepo.GetAllUserPermissions(userId).Result;
                _cache.AddEntriesForUser(userId, userPermissions);
            }

            return Task.CompletedTask;
        }
    }
}