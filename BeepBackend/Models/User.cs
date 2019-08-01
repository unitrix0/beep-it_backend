using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public ICollection<UserArticle> UserArticles { get; set; }
        public ICollection<BeepEnvironment> Environments { get; set; }
        public ICollection<Permission> Permissions { get; set; }
    }
}
