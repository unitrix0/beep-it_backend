using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BeepBackend.Data;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;

namespace BeepBackend.Permissions
{
    public class HasEnvironmentPermissionRequirementHandler : AuthorizationHandler<HasEnvironmentPermissionRequirement>
    {
        private readonly IPermissionsCache _permissionsCache;

        public HasEnvironmentPermissionRequirementHandler(IPermissionsCache cache)
        {
            _permissionsCache = cache;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasEnvironmentPermissionRequirement requirement)
        {
            int userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Permission userPermission = _permissionsCache.GetUserPermission(userId, requirement.EnvironmentId);
            if(userPermission == null) return Task.CompletedTask;

            var userFlags = (PermissionFlags)Convert.ToInt32(userPermission.ToBits(), 2);

            if ((requirement.Permission & userFlags) > 0 ||
                requirement.Permission == 0) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}