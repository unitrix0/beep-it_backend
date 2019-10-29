using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BeepBackend.Helpers
{
    public class HasEnvironmentPermissionHandler : AuthorizationHandler<HasEnvironmentPermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasEnvironmentPermissionRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}