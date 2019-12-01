using System;
using System.Security.Cryptography;

namespace BeepBackend.Permissions
{
    public class SerialGenerator
    {
        public static string Generate()
        {
            var rng = RandomNumberGenerator.Create();
            var randomNumber = new byte[32];

            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}