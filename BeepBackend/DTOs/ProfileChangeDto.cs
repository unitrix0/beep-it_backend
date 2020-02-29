namespace BeepBackend.DTOs
{
    public class ProfileChangeDto
    {
        public string DisplayName { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
        public string CurrentPassword { get; set; }
    }
}