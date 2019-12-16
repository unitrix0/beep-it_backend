using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using BeepBackend.Helpers;
using Microsoft.AspNetCore.Http.Extensions;
using UnitTests.BaseClasses;
using UnitTests.Helper;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class ArticleFilteringTests : DbTestBase
    {
        public ArticleFilteringTests(ITestOutputHelper output, CustomWebApplicationFactory factory) : base(output, factory)
        {
        }

        [Fact]
        public void FilterTest()
        {
            ResetDb();
            WebClient.Login("sepp", "P@ssw0rd");

            var res = WebClient.GetByQuery("https://localhost:5001/api/articles/1", new ArticleFilter()
            {
                IsOpened = true,
                StoreId = 1
            }).Result;
        }

        protected override void ResetDb()
        {
            base.ResetDb();
            SeedAdditionalUser("Sepp");
            SeedAdditionalUser("Tom");
            SeedNewEnvironment("sepp", "Environment2");
            SeedArticleGroup("default");
            SeedArticleGroup("Zahnpasta");

            SeedNewArticle("0001", "Magendarm Tee", "default", "Stk.", "Migros");
            SeedNewArticle("0002", "Schoggimousse", "default", "Stk.", "Coop");
            SeedNewArticle("0003", "Butter", "default", "g", "Aldi");
            SeedNewArticle("0004", "Milch", "default", "l", "Denner");
            SeedNewArticle("0005", "Brot", "default", "Stk.", "Migros");
            SeedNewArticle("0006", "Candida", "Zahnpasta", "Stk.", "Migros");
            SeedNewArticle("0007", "Elmex", "Zahnpasta", "Stk.", "Migros");

            SeedArticleUserSetting("0001", "Zu Hause von Sepp", 0, 1);
            SeedArticleUserSetting("0004", "Zu Hause von Sepp", 1, 1);
            SeedArticleUserSetting("0003", "Zu Hause von Sepp", 1, 1);
            SeedArticleUserSetting("0006", "Zu Hause von Sepp", 1, 2);

            SeedStockEntry("0001", "Zu Hause von Sepp", isOpened: false, amount: 1, openedOn: DateTime.MinValue, expireDate: DateTime.Now.AddDays(28), amountRemaining: 0);

            SeedStockEntry("0004", "Zu Hause von Sepp", isOpened: true, amount: 1, openedOn: DateTime.Now.AddDays(-2), expireDate: DateTime.Now.AddDays(28), amountRemaining: 0.5f);

            SeedStockEntry("0003", "Zu Hause von Sepp", isOpened: true, amount: 1, openedOn: DateTime.Now.AddDays(-5), expireDate: DateTime.Now.AddDays(28), amountRemaining: 0.25f);
            SeedStockEntry("0003", "Zu Hause von Sepp", isOpened: false, amount: 1, openedOn: DateTime.MinValue, expireDate: DateTime.Now.AddDays(40), amountRemaining: 0);

            SeedStockEntry("0006", "Zu Hause von Sepp", isOpened: true, amount: 1, openedOn: DateTime.Now.AddDays(-14), expireDate: DateTime.Now.AddDays(90), amountRemaining: 0.75f);
            SeedStockEntry("0006", "Zu Hause von Sepp", isOpened: false, amount: 1, openedOn: DateTime.MinValue, expireDate: DateTime.Now.AddDays(90), amountRemaining: 0);

            SeedArticleUserSetting("0007", "Zu Hause von Tom", 1, 2);
            SeedStockEntry("0007", "Zu Hause von Tom", true, 1, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(90), 0.75f);
            SeedStockEntry("0007", "Zu Hause von Tom", true, 1, DateTime.MinValue, DateTime.Now.AddDays(90), 0.75f);
        }
    }
}
