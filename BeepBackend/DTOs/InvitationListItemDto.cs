using System;

namespace BeepBackend.DTOs
{
    public class InvitationListItemDto
    {
        public int InviteeId { get; set; }
        public string Invitee { get; set; }
        public string Inviter { get; set; }
        public int EnvironmentId { get; set; }
        public string EnvironmentName { get; set; }
        public DateTime IssuedAt { get; set; }
        public bool IsAnswered { get; set; }
    }
}