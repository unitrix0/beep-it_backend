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

        public BeepBearerEvents(IPermissionsCache cache)
        {
            _cache = cache;
        }

        public override Task TokenValidated(TokenValidatedContext context)
        {
            Dictionary<string, string> claims = context.Principal.Claims.Where(c =>
                    c.Type == BeepClaimTypes.PermissionsSerial ||
                    c.Type == BeepClaimTypes.EnvironmentId)
                .ToDictionary(c => c.Type, c => c.Value);

            string serial = claims.ContainsKey(BeepClaimTypes.PermissionsSerial)
                ? claims[BeepClaimTypes.PermissionsSerial]
                : "";

            int environmentId = claims.ContainsKey(BeepClaimTypes.EnvironmentId)
                ? Convert.ToInt32(claims[BeepClaimTypes.EnvironmentId])
                : 0;

            string userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (_cache.SerialsMatch(userId, environmentId, serial)) return Task.CompletedTask;

            context.Response.Headers.Add("PermissionsChanged", "true");
            return Task.CompletedTask;
        }
    }
}