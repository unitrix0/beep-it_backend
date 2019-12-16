using BeepBackend;
using Microsoft.Extensions.Configuration;

namespace UnitTests.BaseClasses
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
