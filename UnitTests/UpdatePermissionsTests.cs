using BeepBackend.DTOs;
using BeepBackend.Models;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class UpdatePermissionsTests : TestsBase
    {
        public UpdatePermissionsTests(ITestOutputHelper output, CustomWebApplicationFactory factory) : base(output, factory)
        {
            SeedAdditionalUser("Markus");

            JoinEnvironment("Markus", "Zu Hause von Tom", new Permission() { CanScan = true });
            JoinEnvironment("Sepp", "Zu Hause von Tom", new Permission() { CanScan = true, ManageUsers = true });
        }

        [Fact]
        public void ManagerCanChangeMember()
        {
            UserForTokenDto mappedUser = WebClient.Login("sepp", "P@ssw0rd");
            if (mappedUser == null) Assert.False(true);

            HttpResponseMessage result = WebClient.PutAsJsonAsync($"users/Updatepermission", new PermissionsDto()
            {
                UserId = 3,
                EnvironmentId = 2,
                EditArticleSettings = true
            }).Result;
            
            Assert.True(result.IsSuccessStatusCode);
        }
    }
}
