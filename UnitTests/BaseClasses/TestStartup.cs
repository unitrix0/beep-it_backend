using BeepBackend;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace UnitTests.BaseClasses
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }
    }
}
