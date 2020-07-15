﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BeepBackend.Models
{
    public class User : IdentityUser<int>
    {
        public string DisplayName { get; set; }
        public DateTime? AccountExpireDate { get; set; }

        public ICollection<UserArticle> UserArticles { get; set; }
        public ICollection<ArticleGroup> ArticleGroups { get; set; }
        public ICollection<BeepEnvironment> Environments { get; set; }
        public ICollection<Permission> Permissions { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Invitation> InvitedFrom { get; set; }
        public ICollection<UserCamera> Cameras { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
