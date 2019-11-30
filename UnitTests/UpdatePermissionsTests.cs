using BeepBackend.DTOs;
using BeepBackend.Models;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class UpdatePermissionsTests : TestsBase
    {
        private readonly PermissionConversionTests _permissionConversionTests;

        public UpdatePermissionsTests(ITestOutputHelper output, CustomWebApplicationFactory factory) : base(output, factory)
        {
            SeedAdditionalUser("Markus");

            JoinEnvironment("Markus", "Zu Hause von Tom", new Permission() { CanScan = true });
            JoinEnvironment("Sepp", "Zu Hause von Tom", new Permission() { CanScan = true, ManageUsers = true });
            _permissionConversionTests = new PermissionConversionTests();
        }

        [Theory]
        [InlineData(3, 2, true, "CanChangeMember")]
        [InlineData(1, 2, false, "CanNotChangeOwnPermissions")]
        [InlineData(2, 2, false, "CanNotChangeOwnerPermissons")]
        public void ManagerPermissionChange(int userId, int environmentId, bool expectedResult, string comment)
        {
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
    }
}
