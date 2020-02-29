using System;
using System.Text;

namespace BeepBackend.Helpers
{
    public static class StringExtensions
    {
        public static string ToBase64(this string source)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string source)
        {
            var bytes = Convert.FromBase64String(source);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
