using System.IdentityModel.Tokens.Jwt;
using BeepBackend.DTOs;
using BeepBackend.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq;
using System.Net.Http;
using BeepBackend.Helpers;
using Newtonsoft.Json.Linq;
using Utrix.WebLib.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class PermissionsChangedTests : TestsBase
    {
        public PermissionsChangedTests(ITestOutputHelper output, CustomWebApplicationFactory factory)
            : base(output, factory)
        {
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

            var initialPermissions = new Permission() { CanScan = true, EditArticleSettings = true };
            JoinEnvironment("sepp", "Zu Hause von Tom", initialPermissions);

            LoginResponseObject loginSepp = WebClient.Login("sepp", "P@ssw0rd");
            //Switch to other Environment
            HttpResponseMessage changeEnvironmentResult =
                WebClient.PostAsync($"auth/UpdatePermissionClaims/{loginSepp.MappedUser.Id}/?environmentId=2", null).Result;
            loginSepp = JObject.Parse(changeEnvironmentResult.Content.ReadAsStringAsync().Result)
                .ToObject<LoginResponseObject>();

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
            HttpResponseMessage invitationsCountResult = WebClient.UseToken(loginSepp.Token)
                .GetAsync($"users/InvitationsCount/{loginSepp.MappedUser.Id}").Result;

            //Update the Token
            HttpResponseMessage updateTokenResult = WebClient.PostAsync(
                $"auth/UpdatePermissionClaims/{loginSepp.MappedUser.Id}/?environmentId=2", null).Result;
            loginSepp = JObject.Parse(updateTokenResult.Content.ReadAsStringAsync().Result)
                .ToObject<LoginResponseObject>();
            JwtSecurityToken token = JwtHelper.DecodeToken(loginSepp.Token);
            string updatedPermissions = token.Claims.FirstOrDefault(c => c.Type == BeepClaimTypes.Permissions)?.Value;

            //Execute another action to verify permissions changed notification is gone
            HttpResponseMessage noPermissionsChangedHeader = WebClient.UseToken(loginSepp.Token)
                .GetAsync($"users/InvitationsCount/{loginSepp.MappedUser.Id}").Result;


            Assert.True(changeEnvironmentResult.IsSuccessStatusCode);
            Assert.True(setPermissionsResult.IsSuccessStatusCode);

            Assert.NotNull(invitationsCountResult.Headers.FirstOrDefault(h => h.Key == "PermissionsChanged").Value);
            Assert.Equal(newPermission.ToBits(), updatedPermissions);
            Assert.Null(noPermissionsChangedHeader.Headers.FirstOrDefault(h => h.Key == "PermissionsChanged").Value);
        }
    }
}
