using BeepBackend.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using BeepBackend.Data;

namespace BeepBackend.Helpers
{
    public static class Seeder
    {
        public static void Seed(RoleManager<Role> roleMgr)
        {
            CreateRoles(roleMgr);
        }

        private static void CreateRoles(RoleManager<Role> roleMgr)
        {
            if (roleMgr.Roles.Any()) return;
            var roles = new List<Role>()
            {
                new Role() {Name = RoleNames.Admin},
                new Role() {Name = RoleNames.Member}
            };

            foreach (Role role in roles)
            {
                roleMgr.CreateAsync(role).Wait();
            }
        }
    }
}