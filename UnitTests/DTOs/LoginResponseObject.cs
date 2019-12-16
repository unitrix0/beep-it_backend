using BeepBackend.DTOs;

namespace UnitTests.DTOs
{
    internal class LoginResponseObject
    {
        public string Token { get; set; }
        public UserForTokenDto MappedUser { get; set; }
    }
}