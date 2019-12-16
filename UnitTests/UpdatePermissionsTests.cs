using BeepBackend.DTOs;
using BeepBackend.Models;
using System.Net.Http;
using BeepBackend.Permissions;
using UnitTests.BaseClasses;
using UnitTests.DTOs;
using UnitTests.Helper;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class UpdatePermissionsDbTest : DbTestBase
    {
        public UpdatePermissionsDbTest(ITestOutputHelper output, CustomWebApplicationFactory factory) : base(output, factory)
        {
        }

        [Theory]
        [InlineData(3, 2, true, "CanChangeMember")]
        [InlineData(1, 2, false, "CanNotChangeOwnPermissions")]
        [InlineData(2, 2, false, "CanNotChangeOwnerPermissons")]
        public void ManagerPermissionChange(int userId, int environmentId, bool expectedResult, string comment)
        {
            ResetDb();
            OutputWriter.WriteLine(comment);
            LoginResponseObject login = WebClient.Login("sepp", "P@ssw0rd");
            if (login == null) Assert.False(true);

            HttpResponseMessage result = WebClient.PutAsJsonAsync("users/SetPermission", new PermissionsDto()
            {
                UserId = userId,
                EnvironmentId = environmentId,
                EditArticleSettings = true
            }).Result;

            Assert.Equal(expectedResult, result.IsSuccessStatusCode);
        }

        protected override void ResetDb()
        {
            base.ResetDb();
            SeedAdditionalUser("Sepp");
            SeedAdditionalUser("Tom");
            SeedAdditionalUser("Markus");

            JoinEnvironment("Markus", "Zu Hause von Tom", new Permission() { CanScan = true });
            JoinEnvironment("Sepp", "Zu Hause von Tom", new Permission() { CanScan = true, ManageUsers = true });
        }
    }
}
