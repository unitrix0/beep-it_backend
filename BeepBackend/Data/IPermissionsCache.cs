using System;

namespace BeepBackend.Data
{
    public interface IPermissionsCache
    {
        bool SerialsMatch(string identityName, int environmentId, string serial);
        void AddEntry(string key, string serial, DateTime lifetime);
    }
}