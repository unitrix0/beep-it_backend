using Microsoft.AspNetCore.Authorization;

namespace BeepBackend.Helpers
{
    public class HasEnvironmentPermissionRequirement : IAuthorizationRequirement
    {
        public int EnvironmentId { get; set; }
        public byte Permission { get; set; }
    }
}