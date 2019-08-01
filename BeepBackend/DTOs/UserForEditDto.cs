using System.Collections.Generic;

namespace BeepBackend.DTOs
{
    public class UserForEditDto
    {
        public string Username { get; set; }
        public ICollection<EnvironmentDto> Environments { get; set; }
    }
}