using BeepBackend;
using BeepBackend.Data;
using BeepBackend.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly HttpClient _client;

        public UserControllerTests(ITestOutputHelper helper, WebApplicationFactory<Startup> factory)
        {
            _outputHelper = helper;

            WebApplicationFactory<Startup> factoryDelegate = factory.WithWebHostBuilder(builder =>
            {
                IWebHostBuilder host = builder.ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(),
                        "appsettings.json"));
                });
            });

            _client = factoryDelegate.CreateClient();
            IServiceScope scope = factoryDelegate.Server.Host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.Migrate();

        }

        [Fact]
        public async Task Login()
        {
            HttpResponseMessage result = await _client.PostAsJsonAsync("/api/auth/login", new UserForLoginDto()
            {
                Username = "sepp",
                Password = "P@ssw0rd"
            });

            _outputHelper.WriteLine(result.StatusCode.ToString());
            _outputHelper.WriteLine(result.Content.ReadAsStringAsync().Result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void UpdatePermissions()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/updatepermission");

            Assert.Equal(1, 1);
        }
    }
}
