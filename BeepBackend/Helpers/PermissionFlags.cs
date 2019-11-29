using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Helpers
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
