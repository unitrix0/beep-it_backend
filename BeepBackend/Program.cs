using System;
using BeepBackend.Data;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeepBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost host = CreateWebHostBuilder(args).Build();
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<BeepDbContext>();
                    var roleMgr = services.GetRequiredService<RoleManager<Role>>();

                    context.Database.Migrate();
                    Seeder.Seed(roleMgr, context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error during program startup");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        //.UseKestrel((hostBuilder, kestrelOptions) =>
        //{
        //    if (!hostBuilder.HostingEnvironment.IsDevelopment())
        //        return;
        //    kestrelOptions.Listen(IPAddress.Any, 5000);
        //    kestrelOptions.Listen(IPAddress.Any, 5001,
        //        options => options.UseHttps(StoreName.My, "drone02.hive.loc", false,
        //            StoreLocation.LocalMachine));
        //});
    }
}
