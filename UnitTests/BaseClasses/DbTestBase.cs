using BeepBackend;
using BeepBackend.Data;
using BeepBackend.Helpers;
using BeepBackend.Models;
using BeepBackend.Permissions;
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
using UnitTests.Helper;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.BaseClasses
{
    [Collection(DbTestCollection.CollectionName)]
    public class DbTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly RoleManager<Role> _roleMgr;
        protected ITestOutputHelper OutputWriter;
        protected HttpClient WebClient;
        protected UserManager<User> UsrManager;
        protected BeepDbContext DbContext;

        public DbTestBase(ITestOutputHelper output, CustomWebApplicationFactory factory, RamDrive ramDrive)
        {
            OutputWriter = output;
            if(!ramDrive.Ready) throw new Exception($"Ramdrive not ready!: {ramDrive.Error}");
            OutputWriter.WriteLine(ramDrive.Output.ToString());

            WebApplicationFactory<TestStartup> webFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot("BeepBackend");

                builder.ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    string cfgPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                    configurationBuilder.AddJsonFile(cfgPath);
                });

                builder.ConfigureServices(services =>
                {
                    services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
                });
            });

            WebClient = webFactory.CreateClient(
                new WebApplicationFactoryClientOptions() { BaseAddress = new Uri("http://localhost/api/") });

            var scope = factory.Server.Services.CreateScope();
            UsrManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            DbContext = scope.ServiceProvider.GetRequiredService<BeepDbContext>();
            _roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        }

        protected void JoinEnvironment(string userName, string environmentName, Permission withPermission)
        {
            withPermission ??= new Permission();

            User user = DbContext.Users.FirstOrDefault(u => u.UserName == userName);
            BeepEnvironment env = DbContext.Environments.FirstOrDefault(e => e.Name == environmentName);

            if (user == null || env == null) return;

            withPermission.User = user;
            withPermission.Environment = env;
            withPermission.Serial = SerialGenerator.Generate();

            DbContext.Permissions.Add(withPermission);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// Erstellt einen User mit Member Rolle
        /// </summary>
        /// <param name="displayName"></param>
        protected void SeedUser(string displayName)
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
            string confirmationToken = UsrManager.GenerateEmailConfirmationTokenAsync(newUser).Result;
            IdentityResult result = UsrManager.ConfirmEmailAsync(newUser, confirmationToken).Result;
            if (!result.Succeeded) OutputWriter.WriteLine(string.Join("\n", result.Errors));

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

        protected virtual void ResetDb()
        {
            OutputWriter.WriteLine("Resetting DB...");
            DbContext.Database.EnsureDeleted();
            DbContext.Database.Migrate();

            Seeder.Seed(_roleMgr, DbContext);
        }

        protected Article SeedNewArticle(string barcode, string name, string unitAbbreviation, string storeName)
        {
            Store store = DbContext.Stores.FirstOrDefault(s => s.Name == storeName);

            var article = new Article()
            {
                Name = name,
                Barcode = barcode
            };

            DbContext.Articles.Add(article);
            DbContext.ArticleStores.Add(new ArticleStore() { Article = article, Store = store });

            DbContext.SaveChanges();
            return article;
        }


        protected void SeedArticleGroup(string name, int userId)
        {
            DbContext.ArticleGroups.Add(new ArticleGroup() { Name = name });
            DbContext.SaveChanges();
        }

        protected void SeedNewEnvironment(string ownerName, string environmentName)
        {
            User owner = DbContext.Users.FirstOrDefault(u => u.UserName == ownerName.ToLower());

            DbContext.Environments.Add(new BeepEnvironment()
            {
                User = owner,
                Name = environmentName
            });
        }

        /// <summary>
        /// Fügt einen neuen Lager eintrag hinzu
        /// </summary>
        /// <param name="artBarcode">Barcode des Artikels</param>
        /// <param name="environmentName">Name der Umgebung für die der Eintrag gelten soll</param>
        /// <param name="isOpened">true wenn geöffnet</param>
        /// <param name="amount">Anzahl an Lager</param>
        /// <param name="openedOn">Datum an dem der Artikel geöffnet wurde</param>
        /// <param name="expireDate">Ablaufdatum</param>
        /// <param name="amountRemaining">Restmenge wenn ein Artikel geöffnet wird</param>
        protected void SeedStockEntry(string artBarcode, string environmentName, bool isOpened, int amount, DateTime openedOn,
            DateTime expireDate, float amountRemaining)
        {
            Article article = DbContext.Articles.FirstOrDefault(a => a.Barcode == artBarcode);
            BeepEnvironment environment = DbContext.Environments.FirstOrDefault(en => en.Name == environmentName);
            if (article == null || environment == null) throw new Exception("Environment oder Artikel nicht gefunden");

            var stockEntry = DbContext.StockEntries
                                 .FirstOrDefault(se => se.EnvironmentId == environment.Id && se.ArticleId == article.Id) ??
                             new StockEntry() { Article = article, Environment = environment };

            var values = new StockEntryValue()
            {
                StockEntry = stockEntry,
                AmountOnStock = amount,
                AmountRemaining = amountRemaining,
                ExpireDate = expireDate,
                IsOpened = isOpened,
                OpenedOn = openedOn
            };

            if (stockEntry.EnvironmentId == 0) DbContext.StockEntries.Add(stockEntry);
            DbContext.StockEntryValues.Add(values);

            DbContext.SaveChanges();
        }

        protected void SeedArticleUserSetting(string artBarcode, string environmentName, int keepStockAmount,
            int keepStockMode)
        {
            var article = DbContext.Articles.FirstOrDefault(a => a.Barcode == artBarcode);
            var env = DbContext.Environments.FirstOrDefault(e => e.Name == environmentName);

            DbContext.ArticleUserSettings.Add(new ArticleUserSetting()
            {
                Article = article,
                Environment = env,
                KeepStockAmount = keepStockAmount
            });

            DbContext.SaveChanges();
        }



        public void Dispose()
        {
            DbContext.Dispose();
            UsrManager.Dispose();
            _roleMgr.Dispose();
        }
    }
}