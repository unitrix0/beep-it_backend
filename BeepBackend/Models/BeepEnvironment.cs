using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeepBackend.Models
{
    public class BeepEnvironment
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<EnvironmentPermission> EnvironmentPermissions { get; set; }
        public ICollection<ArticleUserSetting> ArticleUserSettings { get; set; }
    }
}