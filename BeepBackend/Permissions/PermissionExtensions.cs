using System;
using System.Collections.Generic;
using BeepBackend.Models;

namespace BeepBackend.Permissions
{
    public static class PermissionExtensions
    {
        public static string ToBits(this Permission permission)
        {
            var values = new List<short>()
            {
                Convert.ToInt16(permission.ManageUsers),
                Convert.ToInt16(permission.EditArticleSettings),
                Convert.ToInt16(permission.CanScan),
                Convert.ToInt16(permission.IsOwner)
            };

            return string.Join("", values);
        }
    }
}
