using System;

namespace BeepBackend.Permissions
{
    [Flags]
    public enum PermissionFlags
    {
        IsOwner = 1 << 0,               // 1
        CanScan = 1 << 1,               // 2
        EditArticleSettings = 1 << 2,   // 4
        ManageUsers = 1 << 3            // 8
    }
}
