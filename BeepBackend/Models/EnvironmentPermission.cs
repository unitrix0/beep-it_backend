namespace BeepBackend.Models
{
    public class EnvironmentPermission
    {
        public int EnvironmentId { get; set; }
        public BeepEnvironment Environment { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}