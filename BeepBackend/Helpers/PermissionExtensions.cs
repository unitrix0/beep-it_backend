using BeepBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BeepBackend.Helpers
{
    public static class PermissionExtensions
    {
        public static string ToBits(this Permission permission)
        {
            var values = new List<short>()
            {
                Convert.ToInt16(permission.IsOwner),
                Convert.ToInt16(permission.CanScan),
                Convert.ToInt16(permission.EditArticleSettings),
                Convert.ToInt16(permission.ManageUsers)
            };

            return string.Join("", values);
        }
    }
}
