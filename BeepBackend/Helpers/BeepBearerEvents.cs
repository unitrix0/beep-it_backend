using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BeepBackend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

            if (_cache.SerialsMatch(userId, environmentId, serial)) return Task.CompletedTask;

            context.Response.Headers.Add("PermissionsChanged", "true");
            return Task.CompletedTask;
        }
    }
}