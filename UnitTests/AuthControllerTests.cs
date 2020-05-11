using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using UnitTests.BaseClasses;
using UnitTests.DTOs;
using UnitTests.Helper;
using Utrix.WebLib.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class AuthControllerTests : DbTestBase
    {
        public AuthControllerTests(ITestOutputHelper output, CustomWebApplicationFactory factory)
            : base(output, factory)
        {
        }
        [Fact]
        public void UpdatePermissionClaims()
        {
            ResetDb();
            JoinEnvironment("fritz", "Zu Hause von Tom", new Permission() { CanScan = true, EditArticleSettings = true });

            var loginSepp = WebClient.Login("sepp", "P@ssw0rd");

            var resultA = WebClient.GetAsyncQuery("auth/UpdatePermissionClaims/1", new { environmentId = 1 }).Result;
            var resultB = WebClient.GetAsyncQuery("auth/UpdatePermissionClaims/1", new { environmentId = 2 }).Result;
            var resultC = WebClient.GetAsyncQuery("auth/UpdatePermissionClaims/3", new { environmentId = 2 }).Result;

            var loginFritz = WebClient.Login("fritz", "P@ssw0rd");
            var resultD = WebClient.GetAsyncQuery("auth/UpdatePermissionClaims/3", new { environmentId = 2 }).Result;

            Assert.NotNull(loginSepp);
            Assert.NotNull(loginFritz);

            Assert.Equal(HttpStatusCode.OK, resultA.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, resultB.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, resultC.StatusCode);
            Assert.Equal(HttpStatusCode.OK, resultD.StatusCode);
        }

        [Fact]
        public void NotificationTest()
        {
            /* 1. Join Env
             * 2. Login user and switch to Tom's env
             * 3. Login Owner
             * 4. Change Permissions
             * 5. Verify Header 
             * 6. Udpate token
             * 7. Verify new Permissions
             * 8. Verify Header*/

            ResetDb();
            var initialPermissions = new Permission() { CanScan = true, EditArticleSettings = true };
            JoinEnvironment("sepp", "Zu Hause von Tom", initialPermissions);

            LoginResponseObject loginSepp = WebClient.Login("sepp", "P@ssw0rd");
            //Switch to other Environment
            HttpResponseMessage changeEnvironmentResult =
                WebClient.GetAsync($"auth/UpdatePermissionClaims/{loginSepp.MappedUser.Id}/?environmentId=2").Result;
            loginSepp.PermissionsToken = JObject.Parse(changeEnvironmentResult.Content.ReadAsStringAsync().Result)
                .ToObject<LoginResponseObject>().PermissionsToken;

            //Set new Permissions
            var newPermission = new PermissionsDto()
            {
                UserId = loginSepp.MappedUser.Id,
                EnvironmentId = 2,
                CanScan = true,
                EditArticleSettings = true,
                ManageUsers = true // New
            };
            WebClient.Login("tom", "P@ssw0rd");
            HttpResponseMessage setPermissionsResult =
                WebClient.PutAsJsonAsync("users/SetPermission", newPermission).Result;

            //Execute some Action to trigger permissions changed notification
            HttpResponseMessage invitationsCountResult = WebClient.UseLogin(loginSepp)
                .GetAsync($"users/InvitationsCount/{loginSepp.MappedUser.Id}").Result;

            //Update the Token
            HttpResponseMessage updateTokenResult = WebClient.GetAsync(
                $"auth/UpdatePermissionClaims/{loginSepp.MappedUser.Id}/?environmentId=2").Result;
            loginSepp.PermissionsToken = JObject.Parse(updateTokenResult.Content.ReadAsStringAsync().Result)
                .ToObject<LoginResponseObject>().PermissionsToken;
            JwtSecurityToken token = JwtHelper.DecodeToken(loginSepp.PermissionsToken);
            string updatedPermissions = token.Claims.FirstOrDefault(c => c.Type == BeepClaimTypes.Permissions)?.Value;

            //Execute another action to verify permissions changed notification is gone
            HttpResponseMessage noPermissionsChangedHeader = WebClient.UseLogin(loginSepp)
                .GetAsync($"users/InvitationsCount/{loginSepp.MappedUser.Id}").Result;


            Assert.True(changeEnvironmentResult.IsSuccessStatusCode);
            Assert.True(setPermissionsResult.IsSuccessStatusCode);

            Assert.NotNull(invitationsCountResult.Headers.FirstOrDefault(h => h.Key == "permissions_changed").Value);
            Assert.Equal(newPermission.ToBits(), updatedPermissions);
            Assert.Null(noPermissionsChangedHeader.Headers.FirstOrDefault(h => h.Key == "permissions_changed").Value);
        }

        protected override void ResetDb()
        {
            base.ResetDb();

            SeedUser("Sepp");
            SeedUser("Tom");
            SeedUser("Fritz");
        }
    }
}
