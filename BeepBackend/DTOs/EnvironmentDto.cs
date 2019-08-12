using System.Collections.Generic;

namespace BeepBackend.DTOs
{
    public class EnvironmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PermissionsDto> Permissions { get; set; }
    }
}