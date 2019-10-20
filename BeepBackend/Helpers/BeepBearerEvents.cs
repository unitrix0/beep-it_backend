using System;
using System.Collections.Generic;
using System.Linq;
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
            var claims = context.Principal.Claims.Where(c =>
                    c.Type == BeepClaimTypes.PermissionsSerial ||
                    c.Type == BeepClaimTypes.EnvironmentId)
                .ToDictionary(c => c.Type, c => c.Value);

            string serial = claims.ContainsKey(BeepClaimTypes.PermissionsSerial)
                ? claims[BeepClaimTypes.PermissionsSerial]
                : "";

            int environmentId = claims.ContainsKey(BeepClaimTypes.EnvironmentId)
                ? Convert.ToInt32(claims[BeepClaimTypes.EnvironmentId])
                : 0;

            if (_cache.SerialsMatch(context.Principal.Identity.Name, environmentId, serial)) return Task.CompletedTask;

            context.Response.Headers.Add("PermissionsChanged", "true");
            return Task.CompletedTask;
        }
    }
}