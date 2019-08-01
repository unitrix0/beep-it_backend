using System.Collections.Generic;

namespace BeepBackend.DTOs
{
    public class EnvironmentDto
    {
        public string Name { get; set; }
        public ICollection<UserWithPermissionsDto> Users { get; set; }
    }
}