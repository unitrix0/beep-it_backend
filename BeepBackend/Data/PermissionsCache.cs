using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BeepBackend.Data
{
    public class PermissionsCache : IPermissionsCache
    {
        private readonly Dictionary<string, Tuple<string, DateTime>> _serialsCache;

        public PermissionsCache()
        {
            _serialsCache = new Dictionary<string, Tuple<string, DateTime>>();

            var cleanupTimer = new Timer(Cleanup);
            cleanupTimer.Change(new TimeSpan(), new TimeSpan(1, 0, 0, 0));
        }

        public bool SerialsMatch(string identityName, int environmentId, string serial)
        {
            string key = $"{identityName},{environmentId}";
            return _serialsCache.ContainsKey(key) && _serialsCache[key].Item1 == serial;
        }

        public void AddEntry(string key, string serial, DateTime lifetime)
        {
            if (_serialsCache.ContainsKey(key)) _serialsCache.Remove(key);
            _serialsCache.Add(key, new Tuple<string, DateTime>(serial, lifetime));
        }

        public void Update(string userUserName, int environmentId, string serial)
        {
            string key = $"{userUserName},{environmentId}";
            if (!_serialsCache.ContainsKey(key)) return;

            Tuple<string, DateTime> current = _serialsCache[key];
            _serialsCache.Remove(key);
            _serialsCache.Add(key, new Tuple<string, DateTime>(serial, current.Item2));
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