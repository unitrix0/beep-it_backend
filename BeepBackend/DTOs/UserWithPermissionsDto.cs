using System.Collections.Generic;

namespace BeepBackend.DTOs
{
    public class UserWithPermissionsDto
    {
        public string Username { get; set; }
        public PermissionsDto Permissions { get; set; }
    }
}