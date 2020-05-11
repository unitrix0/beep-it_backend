using BeepBackend.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeepBackend.Permissions
{
    public class PermissionsCacheCleanup : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private Timer _timer;
        private int _interval;
        private readonly IConfigurationSection _appSettings;

        public PermissionsCacheCleanup(IServiceProvider services, IConfiguration config)
        {
            _services = services;
            _appSettings = config.GetSection("AppSettings");
            _interval = Convert.ToInt32(_appSettings["PermissionsCacheCleanupInterval"]);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TimerWork, null, TimeSpan.FromSeconds(_interval), TimeSpan.FromSeconds(_interval));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void TimerWork(object state)
        {
            using (var scope = _services.CreateScope())
            {
                Console.WriteLine("Permissions Cache Cleanup");

                var permissionsCache = scope.ServiceProvider
                    .GetRequiredService<IPermissionsCache>();
                permissionsCache.Cleanup();

                var newInterval = Convert.ToInt32(_appSettings["PermissionsCacheCleanupInterval"]);
                if(newInterval == _interval) return;

                _interval = newInterval;
                Console.WriteLine($"Chaning PermissionsCacheCleanupInterval to: {_interval}s");
                _timer.Change(TimeSpan.FromSeconds(_interval), TimeSpan.FromSeconds(_interval));
            }

        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
