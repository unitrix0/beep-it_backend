using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeepBackend.Models
{
    public class BeepEnvironment
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool DefaultEnvironment { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Permission> Permissions { get; set; }
        public ICollection<ArticleUserSetting> ArticleUserSettings { get; set; }
        public ICollection<Invitation> Invitations { get; set; }
        public ICollection<StockEntry> StockEntries { get; set; }
    }
}