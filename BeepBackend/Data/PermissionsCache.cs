using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BeepBackend.Models;

namespace BeepBackend.Data
{
    public class PermissionsCache : IPermissionsCache
    {
        private readonly Dictionary<string, Tuple<Permission, DateTime>> _serialsCache;

        public PermissionsCache()
        {
            _serialsCache = new Dictionary<string, Tuple<Permission, DateTime>>();

            var cleanupTimer = new Timer(Cleanup);
            cleanupTimer.Change(new TimeSpan(), new TimeSpan(1, 0, 0, 0));
        }

        public bool SerialsMatch(string userId, int environmentId, string permissionSerial)
        {
            string key = $"{userId},{environmentId}";
            return _serialsCache.ContainsKey(key) && _serialsCache[key].Item1.Serial == permissionSerial;
        }

        public void AddEntry(int userId, int environmentId, Permission permission, DateTime lifetime)
        {
            string key = $"{userId},{environmentId}";

            if (_serialsCache.ContainsKey(key)) _serialsCache.Remove(key);
            _serialsCache.Add(key, new Tuple<Permission, DateTime>(permission, lifetime));
        }

        public void Update(int userId, int environmentId, Permission permission)
        {
            string key = $"{userId},{environmentId}";
            if (!_serialsCache.ContainsKey(key)) return;

            Tuple<Permission, DateTime> current = _serialsCache[key];
            _serialsCache.Remove(key);
            _serialsCache.Add(key, new Tuple<Permission, DateTime>(permission, current.Item2));
        }

        public Permission GetUserPermission(int userId, int environmentId)
        {
            string key = $"{userId},{environmentId}";
            return _serialsCache.ContainsKey(key) ? _serialsCache[key].Item1 : new Permission();
        }

        private void Cleanup(object state)
        {
            IEnumerable<string> oldKeys = _serialsCache.Where(c => c.Value.Item2 < DateTime.Now).Select(c => c.Key);
            foreach (string oldKey in oldKeys)
            {
                _serialsCache.Remove(oldKey);
            }
        }
    }
}