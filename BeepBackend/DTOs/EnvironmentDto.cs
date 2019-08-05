using System.Collections.Generic;

namespace BeepBackend.DTOs
{
    public class EnvironmentDto
    {
        public string Name { get; set; }
        public ICollection<PermissionsDto> Permissions { get; set; }
    }
}