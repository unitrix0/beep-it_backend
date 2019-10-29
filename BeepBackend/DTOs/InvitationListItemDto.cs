using System;

namespace BeepBackend.DTOs
{
    public class InvitationListItemDto
    {
        public string Inviter { get; set; }
        public string EnvironmentName { get; set; }
        public int EnvironmentId { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}