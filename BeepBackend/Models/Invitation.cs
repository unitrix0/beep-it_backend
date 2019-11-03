using System;
using System.ComponentModel.DataAnnotations;

namespace BeepBackend.Models
{
    public class Invitation
    {
        public int InviteeId { get; set; }
        public User Invitee { get; set; }

        public int EnvironmentId { get; set; }
        public BeepEnvironment Environment { get; set; }

        public string Serial { get; set; }
        [Required]
        public DateTime IssuedAt { get; set; }

        public DateTime AnsweredOn { get; set; }
    }
}