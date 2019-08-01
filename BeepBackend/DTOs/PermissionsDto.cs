namespace BeepBackend.DTOs
{
    public class PermissionsDto
    {
        public int EnvironmentId { get; set; }
        public bool IsOwner { get; set; }
        public bool CanView { get; set; }
        public bool CheckIn { get; set; }
        public bool CheckOut { get; set; }
        public bool EditArticleSettings { get; set; }
        public bool Invite { get; set; }
        public bool RemoveMember { get; set; }
    }
}