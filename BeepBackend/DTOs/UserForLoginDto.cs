using System.Collections.Generic;

namespace BeepBackend.DTOs
{
    public class UserForLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public IEnumerable<CameraDto> Cameras { get; set; }
    }
}