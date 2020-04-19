using BeepBackend.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BeepBackend.Data
{
    public class PermissionsCache : IPermissionsCache
    {

        private readonly ConcurrentDictionary<string, Tuple<Permission, DateTime>> _serialsCache;
        private readonly int _tokenLifeTimeSeconds;

        public PermissionsCache(IConfiguration config)
        {
            _serialsCache = new ConcurrentDictionary<string, Tuple<Permission, DateTime>>();
            _tokenLifeTimeSeconds = Convert.ToInt32(config.GetSection("AppSettings:TokenLifeTime").Value);

            var cleanupTimer = new Timer(Cleanup);
            cleanupTimer.Change(new TimeSpan(), new TimeSpan(1, 0, 0, 0));
        }

        public PermissionsChacheResult SerialsMatch(int userId, int environmentId, string permissionSerial)
        {
            string key = $"{userId},{environmentId}";

            if (!_serialsCache.ContainsKey(key)) return PermissionsChacheResult.NotCached;
            return _serialsCache[key].Item1.Serial == permissionSerial ? PermissionsChacheResult.DoMatch:PermissionsChacheResult.DoNotMatch;
        }

        public void AddEntriesForUser(int userId, IEnumerable<Permission> permissions)
        {
            foreach (Permission p in permissions)
            {
                string key = $"{userId},{p.EnvironmentId}";
                var value = new Tuple<Permission, DateTime>(p, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds));

                _serialsCache.AddOrUpdate(key, value, (k, v) => value);
            }
        }

        public void Update(int userId, int environmentId, Permission permission)
        {
            string key = $"{userId},{environmentId}";
            var value = new Tuple<Permission, DateTime>(permission, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds));
            _serialsCache.AddOrUpdate(key, value, (k, v) => new Tuple<Permission, DateTime>(permission, v.Item2));
        }

        public Permission GetUserPermission(int userId, int environmentId)
        {
            string key = $"{userId},{environmentId}";
            return _serialsCache.ContainsKey(key) ? _serialsCache[key].Item1 : null;
        }

        private void Cleanup(object state)
        {
            IEnumerable<string> oldKeys = _serialsCache.Where(c => c.Value.Item2 < DateTime.Now).Select(c => c.Key);
            foreach (string oldKey in oldKeys)
            {
                _serialsCache.Remove(oldKey, out _);
            }
        }
    }
}