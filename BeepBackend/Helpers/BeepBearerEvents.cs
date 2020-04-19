using BeepBackend.Data;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
            switch (chacheResult)
            {
                case PermissionsChacheResult.DoNotMatch:
                    context.Response.Headers.Add("PermissionsChanged", "true");
                    break;
                case PermissionsChacheResult.NotCached:
                    IEnumerable<Permission> userPermissions = _authRepo.GetAllUserPermissions(userId).Result;
                    _cache.AddEntriesForUser(userId, userPermissions);
                    //return TokenValidated(context);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}