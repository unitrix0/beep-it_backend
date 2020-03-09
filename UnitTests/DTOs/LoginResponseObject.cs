using BeepBackend.DTOs;

namespace UnitTests.DTOs
{
    internal class LoginResponseObject
    {
        public string IdentityToken { get; set; }
        public string PermissionsToken { get; set; }
        public UserForTokenDto MappedUser { get; set; }
    }
}