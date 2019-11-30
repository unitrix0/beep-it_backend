using BeepBackend.DTOs;

namespace UnitTests
{
    internal class LoginResponseObject
    {
        public string Token { get; set; }
        public UserForTokenDto MappedUser { get; set; }
    }
}