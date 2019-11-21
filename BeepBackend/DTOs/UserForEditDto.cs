using System.Collections.Generic;

namespace BeepBackend.DTOs
{
    public class UserForEditDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public ICollection<EnvironmentDto> Environments { get; set; }
    }
}