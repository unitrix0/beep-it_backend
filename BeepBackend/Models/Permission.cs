using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeepBackend.Models
{
    public class Permission
    {
        public bool IsOwner { get; set; }
        public bool CanView { get; set; }
        public bool CheckIn { get; set; }
        public bool CheckOut { get; set; }
        public bool EditArticleSettings { get; set; }
        public bool Invite { get; set; }
        public bool RemoveMember { get; set; }
        [Required]
        public string Serial { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int EnvironmentId { get; set; }
        public BeepEnvironment Environment { get; set; }
    }
}