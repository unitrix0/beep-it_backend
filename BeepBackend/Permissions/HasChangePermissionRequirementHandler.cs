using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BeepBackend.Data;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;

namespace BeepBackend.Permissions
{
    public class HasChangePermissionRequirementHandler : AuthorizationHandler<ChangePermissionRequirement>
    {
        private readonly IPermissionsCache _permissionsCache;

        public HasChangePermissionRequirementHandler(IPermissionsCache permissionsCache)
        {
            _permissionsCache = permissionsCache;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ChangePermissionRequirement requirement)
        {
            int userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Permission userPermission = _permissionsCache.GetUserPermission(userId, requirement.EnvironmentId);
            var userFlags = (PermissionFlags)Convert.ToInt32(userPermission.ToBits(), 2);

            if((requirement.PermissionsUserId == userId ||
                requirement.PermissionsUserId == requirement.EnvironmentOwnerId) &&
               !userFlags.HasFlag(PermissionFlags.IsOwner)) return Task.CompletedTask;

            if ((requirement.Permission & userFlags) > 0) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}