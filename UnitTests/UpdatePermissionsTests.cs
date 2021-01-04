using System.Net.Http;
using BeepBackend.DTOs;
using BeepBackend.Models;
using UnitTests.BaseClasses;
using UnitTests.Helper;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class UpdatePermissionsDbTest : DbTestBase
    {
        public UpdatePermissionsDbTest(ITestOutputHelper output, CustomWebApplicationFactory factory, RamDrive ramDrive) : base(output,
            factory, ramDrive)
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
            var login = WebClient.Login("sepp", "P@ssw0rd");

            var result = WebClient.PutAsJsonAsync("users/SetPermission", new PermissionsDto()
            {
                UserId = userId,
                EnvironmentId = environmentId,
                EditArticleSettings = true
            }).Result;

            Assert.NotNull(login);
            Assert.Equal(expectedResult, result.IsSuccessStatusCode);
        }

        protected override void ResetDb()
        {
            base.ResetDb();
            SeedUser("Sepp");
            SeedUser("Tom");
            SeedUser("Markus");

            JoinEnvironment("Markus", "Zu Hause von Tom", new Permission() {CanScan = true});
            JoinEnvironment("Sepp", "Zu Hause von Tom", new Permission() {CanScan = true, ManageUsers = true});
        }
    }
}