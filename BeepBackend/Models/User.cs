using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BeepBackend.Models
{
    public class User : IdentityUser<int>
    {
        public string DisplayName { get; set; }
        public byte[] PasswordSalt { get; set; }

        public ICollection<UserArticle> UserArticles { get; set; }
        public ICollection<BeepEnvironment> Environments { get; set; }
        public ICollection<Permission> Permissions { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
