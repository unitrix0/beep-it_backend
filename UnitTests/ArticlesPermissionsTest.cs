using BeepBackend.DTOs;
using BeepBackend.Models;
using System;
using System.Net;
using System.Net.Http;
using UnitTests.BaseClasses;
using UnitTests.DTOs;
using UnitTests.Helper;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class ArticlesPermissionsTest : DbTestBase
    {
        public ArticlesPermissionsTest(ITestOutputHelper output, CustomWebApplicationFactory factory) : base(output, factory)
        {
        }

        [Fact]
        public void GetArticlesNoMember()
        {
            ResetDb();
            LoginResponseObject login = WebClient.Login("sepp", "P@ssw0rd");
            HttpResponseMessage resultA =
                WebClient.GetAsyncQuery("articles/GetArticles", new { EnvironmentId = 2 }).Result;
            HttpResponseMessage resultB =
                WebClient.GetAsyncQuery("articles/GetArticles", new { EnvironmentId = 1 }).Result;

            LoginResponseObject loginFritz = WebClient.Login("fritz", "P@ssw0rd");
            HttpResponseMessage resultC =
                WebClient.GetAsyncQuery("articles/GetArticles", new { EnvironmentId = 2 }).Result;
            HttpResponseMessage resultD =
                WebClient.GetAsyncQuery("articles/GetArticles", new { EnvironmentId = 1 }).Result;


            Assert.NotNull(login);
            Assert.NotNull(loginFritz);
            Assert.Equal(HttpStatusCode.Unauthorized, resultA.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, resultC.StatusCode);
            Assert.Equal(HttpStatusCode.OK, resultB.StatusCode);
            Assert.Equal(HttpStatusCode.OK, resultD.StatusCode);
        }

        [Fact]
        public void GetBaseDataWhenLoggedIn()
        {
            ResetDb();

            HttpResponseMessage resultA = WebClient.GetAsync("articles/GetBaseData/99").Result;

            LoginResponseObject login = WebClient.Login("sepp", "P@ssw0rd");
            HttpResponseMessage resultB = WebClient.GetAsync("articles/GetBaseData/2").Result;

            Assert.NotNull(login);
            Assert.Equal(HttpStatusCode.OK, resultB.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, resultA.StatusCode);
        }

        [Fact]
        public void GetArticleDateSuggestion()
        {
            ResetDb();

            SeedNewArticle("9999", "Dummy", "Stk.", "Migros");
            SeedArticleUserSetting("9999", "Zu Hause von Sepp", 1, 1);
            SeedStockEntry("9999", "Zu Hause von Sepp", false, 1, DateTime.Now, DateTime.Now, 1);

            LoginResponseObject login = WebClient.Login("sepp", "P@ssw0rd");
            HttpResponseMessage resultA = WebClient.GetAsync("articles/GetArticleDateSuggestions/9999/2").Result;
            HttpResponseMessage resultB = WebClient.GetAsync("articles/GetArticleDateSuggestions/9999/1").Result;

            LoginResponseObject loginFritz = WebClient.Login("fritz", "P@ssw0rd");
            HttpResponseMessage resultC = WebClient.GetAsync("articles/GetArticleDateSuggestions/9999/2").Result;
            HttpResponseMessage resultD = WebClient.GetAsync("articles/GetArticleDateSuggestions/9999/1").Result;

            Assert.NotNull(login);
            Assert.NotNull(loginFritz);
            Assert.Equal(HttpStatusCode.Unauthorized, resultA.StatusCode);
            Assert.Equal(HttpStatusCode.OK, resultB.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, resultC.StatusCode);
            Assert.Equal(HttpStatusCode.OK, resultD.StatusCode);
        }

        [Fact]
        public void GetArticleStock()
        {
            ResetDb();

            var queryParamsA = new
            {
                articleId = "9999",
                environmentId = "2",
                pageNumber = "1",
                itemsPerPage = "10"
            };
            var queryParamsB = new
            {
                articleId = "9999",
                environmentId = "1",
                pageNumber = "1",
                itemsPerPage = "10"
            };

            LoginResponseObject login = WebClient.Login("sepp", "P@ssw0rd");
            HttpResponseMessage resultA = WebClient.GetAsyncQuery("articles/GetArticleStock/", queryParamsA).Result;
            HttpResponseMessage resultB = WebClient.GetAsyncQuery("articles/GetArticleStock/", queryParamsB).Result;

            LoginResponseObject loginFritz = WebClient.Login("fritz", "P@ssw0rd");
            HttpResponseMessage resultC = WebClient.GetAsyncQuery("articles/GetArticleStock/", queryParamsA).Result;
            HttpResponseMessage resultD = WebClient.GetAsyncQuery("articles/GetArticleStock/", queryParamsB).Result;

            Assert.NotNull(login);
            Assert.NotNull(loginFritz);
            Assert.Equal(HttpStatusCode.Unauthorized, resultA.StatusCode);
            Assert.Equal(HttpStatusCode.OK, resultB.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, resultC.StatusCode);
            Assert.Equal(HttpStatusCode.OK, resultD.StatusCode);

        }

        [Fact]
        public void CheckOutById()
        {
            ResetDb();
            SeedNewArticle("9999", "xxx", "Stk.", "Migros");
            SeedArticleUserSetting("9999", "Zu Hause von Tom", 1, 1);
            SeedStockEntry("9999", "Zu Hause von Tom", false, 2, DateTime.Now, DateTime.Now, 1);

            var queryObj = new
            {
                entryId = "1",
                amount = "1"
            };

            LoginResponseObject loginSepp = WebClient.Login("sepp", "P@ssw0rd");
            HttpResponseMessage resultA = WebClient.DeleteAsyncQuery("articles/CheckOutById", queryObj).Result;

            LoginResponseObject loginFritz = WebClient.Login("fritz", "P@ssw0rd");
            HttpResponseMessage resultB = WebClient.DeleteAsyncQuery("articles/CheckOutById", queryObj).Result;

            LoginResponseObject loginTom = WebClient.Login("tom", "P@ssw0rd");
            HttpResponseMessage resultC = WebClient.DeleteAsyncQuery("articles/CheckOutById", queryObj).Result;

            Assert.NotNull(loginSepp);
            Assert.NotNull(loginTom);
            Assert.NotNull(loginFritz);
            Assert.Equal(HttpStatusCode.Unauthorized, resultA.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, resultB.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, resultC.StatusCode);
        }

        [Fact]
        public void OpenArticle()
        {
            ResetDb();
            SeedNewArticle("9999", "xxx", "Stk.", "Migros");
            SeedArticleUserSetting("9999", "Zu Hause von Tom", 1, 1);
            SeedStockEntry("9999", "Zu Hause von Tom", false, 2, DateTime.Now, DateTime.Now, 1);

            var sevDto = new StockEntryValueDto
            {
                Id = 1,
                EnvironmentId = 2,
                AmountOnStock = 2,
                ExpireDate = DateTime.Now,
                ArticleId = 1,
                IsOpened = false,
                OpenedOn = DateTime.Now,
                AmountRemaining = 0.5f,
                ClientTimezoneOffset = 0
            };

            LoginResponseObject loginSepp = WebClient.Login("sepp", "P@ssw0rd");
            HttpResponseMessage resultA = WebClient.PutAsJsonAsync("articles/OpenArticle", sevDto).Result;

            LoginResponseObject loginFritz = WebClient.Login("fritz", "P@ssw0rd");
            HttpResponseMessage resultB = WebClient.PutAsJsonAsync("articles/OpenArticle", sevDto).Result;

            LoginResponseObject loginTom = WebClient.Login("tom", "P@ssw0rd");
            HttpResponseMessage resultC = WebClient.PutAsJsonAsync("articles/OpenArticle", sevDto).Result;

            Assert.NotNull(loginSepp);
            Assert.NotNull(loginTom);
            Assert.NotNull(loginFritz);
            Assert.Equal(HttpStatusCode.Unauthorized, resultA.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, resultB.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, resultC.StatusCode);
        }

        protected override void ResetDb()
        {
            base.ResetDb();

            SeedUser("Sepp");
            SeedUser("Tom");

            SeedUser("Fritz");
            JoinEnvironment("fritz", "Zu Hause von Sepp", new Permission()
            {
                CanScan = true,
                EditArticleSettings = true,
                ManageUsers = true
            });
        }
    }
}
