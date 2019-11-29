using BeepBackend.Data;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeepBackend.Helpers
{
    public class HasEnvironmentPermissionHandler : AuthorizationHandler<HasEnvironmentPermissionRequirement>
    {
        private readonly IPermissionsCache _permissionsCache;

        public HasEnvironmentPermissionHandler(IPermissionsCache permissionsCache)
        {
            _permissionsCache = permissionsCache;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasEnvironmentPermissionRequirement requirement)
        {
            int userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Permission userPermission = _permissionsCache.GetUserPermission(userId, requirement.EnvironmentId);
            var userFlags = (PermissionFlags)Convert.ToInt32(userPermission.ToBits(), 2);

            if(requirement.PermissionsUserId == requirement.EnvironmentOwnerId && 
               userId != requirement.EnvironmentOwnerId) return Task.CompletedTask;

            if ((requirement.Permission & userFlags) > 0) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}