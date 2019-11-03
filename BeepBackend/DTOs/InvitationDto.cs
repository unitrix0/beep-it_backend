namespace BeepBackend.DTOs
{
    public class InvitationDto
    {
        public string InviteeName { get; set; }
        public int EnvironmentId { get; set; }
        public bool SendMail { get; set; }
    }
}