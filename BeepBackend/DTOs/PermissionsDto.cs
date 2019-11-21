namespace BeepBackend.DTOs
{
    public class PermissionsDto
    {
        public int EnvironmentId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }

        public bool IsOwner { get; set; }
        public bool CanScan { get; set; }
        public bool EditArticleSettings { get; set; }
        public bool ManageUsers { get; set; }
        
    }
}