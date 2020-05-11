using BeepBackend.Models;
using BeepBackend.Permissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BeepBackend.Helpers
{
    public class DemoUserCleanup : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private Timer _timer;
        private readonly IConfigurationSection _appSettings;
        private int _interval;

        public DemoUserCleanup(IServiceProvider services, IConfiguration config)
        {
            _services = services;
            _appSettings = config.GetSection("AppSettings");
            _interval = Convert.ToInt32(_appSettings["DemoUserCleanupInterval"]);
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
            Console.WriteLine("Demo-User Cleanup Running");
            using (var scope = _services.CreateScope())
            {
                UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                IList<User> users = userManager.GetUsersInRoleAsync(RoleNames.Demo).Result;
                foreach (User user in users)
                {
                    if (!user.AccountExpireDate.HasValue || user.AccountExpireDate.Value > DateTime.Now) continue;

                    Console.WriteLine($"Deleting User: {user.UserName} ({user.Id})");
                    userManager.DeleteAsync(user).Wait();
                }

                var newInterval = Convert.ToInt32(_appSettings["DemoUserCleanupInterval"]);
                if (newInterval == _interval) return;

                _interval = newInterval;
                Console.WriteLine($"Chaning DemoUserCleanupInterval to: {_interval}s");
                _timer.Change(TimeSpan.FromSeconds(_interval), TimeSpan.FromSeconds(_interval));
            }


        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
