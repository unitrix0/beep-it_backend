using BeepBackend;
using BeepBackend.Data;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class TestsBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected ITestOutputHelper OutputWriter;
        protected HttpClient WebClient;
        protected UserManager<User> UsrManager;
        protected BeepDbContext DbContext;
        private readonly RoleManager<Role> _roleMgr;

        public TestsBase(ITestOutputHelper output, WebApplicationFactory<TestStartup> factory)
        {
            OutputWriter = output;
            string cfgPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

            WebApplicationFactory<TestStartup> webFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot("BeepBackend");

                builder.ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder.AddJsonFile(cfgPath);
                });

                builder.ConfigureServices(services =>
                {
                    services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
                });
            });

            WebClient = webFactory.CreateClient(
                new WebApplicationFactoryClientOptions() {BaseAddress = new Uri("http://localhost/api/")});
            UsrManager = webFactory.Server.Host.Services.GetRequiredService<UserManager<User>>();
            DbContext = webFactory.Server.Host.Services.GetRequiredService<BeepDbContext>();
            _roleMgr = webFactory.Server.Host.Services.GetRequiredService<RoleManager<Role>>();
            ResetDb();
        }

        protected void JoinEnvironment(string userName, string environmentName, Permission withPermission)
        {
            if (withPermission == null) withPermission = new Permission();

            User user = DbContext.Users.FirstOrDefault(u => u.UserName == userName);
            BeepEnvironment env = DbContext.Environments.FirstOrDefault(e => e.Name == environmentName);

            if (user == null || env == null) return;

            withPermission.User = user;
            withPermission.Environment = env;
            withPermission.Serial = SerialGenerator.Generate();

            DbContext.Permissions.Add(withPermission);
            DbContext.SaveChanges();
        }

        protected void SeedAdditionalUser(string displayName)
        {
            var newUser = new User()
            {
                UserName = displayName.ToLower().Replace(" ", ""),
                DisplayName = displayName,
                Email = $"{displayName.ToLower().Replace(" ", "")}@abc.ch"
            };
            var newEnvironment = new BeepEnvironment()
            {
                Name = $"Zu Hause von {displayName}",
                DefaultEnvironment = true,
                User = newUser
            };

            UsrManager.CreateAsync(newUser, "P@ssw0rd").Wait();
            UsrManager.AddToRoleAsync(newUser, RoleNames.Member).Wait();
            DbContext.Environments.Add(newEnvironment);
            DbContext.Permissions.Add(new Permission()
            {
                IsOwner = true,
                User = newUser,
                Environment = newEnvironment,
                Serial = SerialGenerator.Generate()
            });

            DbContext.SaveChanges();
        }

        protected void ResetDb()
        {
            OutputWriter.WriteLine("Resetting DB...");
            DbContext.Database.EnsureDeleted();
            DbContext.Database.Migrate();

            Seeder.Seed(_roleMgr);

            var sepp = new User { UserName = "sepp", DisplayName = "Sepp", Email = "sepp@abc.ch" };
            var tom = new User { UserName = "tom", DisplayName = "Tom", Email = "tom@abc.ch" };

            var seppEnvironment = new BeepEnvironment { Name = "Zu Hause von Sepp", DefaultEnvironment = true, User = sepp };
            var tomEnvironment = new BeepEnvironment { Name = "Zu Hause von Tom", DefaultEnvironment = true, User = tom };

            UsrManager.CreateAsync(sepp, "P@ssw0rd").Wait();
            UsrManager.CreateAsync(tom, "P@ssw0rd").Wait();
            UsrManager.AddToRoleAsync(sepp, RoleNames.Member).Wait();
            UsrManager.AddToRoleAsync(tom, RoleNames.Member).Wait();
            DbContext.Environments.Add(seppEnvironment);
            DbContext.Environments.Add(tomEnvironment);
            DbContext.Permissions.Add(new Permission { IsOwner = true, User = sepp, Environment = seppEnvironment, Serial = SerialGenerator.Generate() });
            DbContext.Permissions.Add(new Permission { IsOwner = true, User = tom, Environment = tomEnvironment, Serial = SerialGenerator.Generate() });

            DbContext.SaveChanges();
        }
    }
}