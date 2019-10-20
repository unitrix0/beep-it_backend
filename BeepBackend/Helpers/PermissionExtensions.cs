using BeepBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeepBackend.Helpers
{
    public static class PermissionExtensions
    {
        public static string ToBits(this Permission permission)
        {
            List<short> values = typeof(Permission).GetProperties()
                .Where(p => p.PropertyType == typeof(bool))
                .Select(pi => Convert.ToInt16(pi.GetValue(permission)))
                .ToList();

            while (values.Count % 8 != 0)
            {
                values.Insert(0, 0);
            }

            return string.Join("", values);
        }
    }
}
