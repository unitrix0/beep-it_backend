using System;
using UnitTests.BaseClasses;
using UnitTests.Helper;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class ArticleFilteringTests : DbTestBase
    {
        public ArticleFilteringTests(ITestOutputHelper output, CustomWebApplicationFactory factory, RamDrive ramDrive) : base(output,
            factory, ramDrive)
        {
        }

        protected override void ResetDb()
        {
            base.ResetDb();
            SeedUser("Sepp");
            SeedUser("Tom");
            SeedNewEnvironment("sepp", "Environment2");
            //SeedArticleGroup("default");
            //SeedArticleGroup("Zahnpasta");

            SeedNewArticle("0001", "Magendarm Tee", "Stk.", "Migros");
            SeedNewArticle("0002", "Schoggimousse", "Stk.", "Coop");
            SeedNewArticle("0003", "Butter", "g", "Aldi");
            SeedNewArticle("0004", "Milch", "l", "Denner");
            SeedNewArticle("0005", "Brot", "Stk.", "Migros");
            SeedNewArticle("0006", "Candida", "Stk.", "Migros");
            SeedNewArticle("0007", "Elmex", "Stk.", "Migros");

            SeedArticleUserSetting("0001", "Zu Hause von Sepp", 0, 1);
            SeedArticleUserSetting("0004", "Zu Hause von Sepp", 1, 1);
            SeedArticleUserSetting("0003", "Zu Hause von Sepp", 1, 1);
            SeedArticleUserSetting("0006", "Zu Hause von Sepp", 1, 2);

            SeedStockEntry("0001", "Zu Hause von Sepp", false, 1, DateTime.MinValue, DateTime.Now.AddDays(28), 0);

            SeedStockEntry("0004", "Zu Hause von Sepp", true, 1, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(28),
                0.5f);

            SeedStockEntry("0003", "Zu Hause von Sepp", true, 1, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(28),
                0.25f);
            SeedStockEntry("0003", "Zu Hause von Sepp", false, 1, DateTime.MinValue, DateTime.Now.AddDays(40), 0);

            SeedStockEntry("0006", "Zu Hause von Sepp", true, 1, DateTime.Now.AddDays(-14), DateTime.Now.AddDays(90),
                0.75f);
            SeedStockEntry("0006", "Zu Hause von Sepp", false, 1, DateTime.MinValue, DateTime.Now.AddDays(90), 0);

            SeedArticleUserSetting("0007", "Zu Hause von Tom", 1, 2);
            SeedStockEntry("0007", "Zu Hause von Tom", true, 1, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(90),
                0.75f);
            SeedStockEntry("0007", "Zu Hause von Tom", true, 1, DateTime.MinValue, DateTime.Now.AddDays(90), 0.75f);
        }
    }
}