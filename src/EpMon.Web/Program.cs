using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace EpMon.Web.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .CaptureStartupErrors(true) // the default
                .UseSetting("detailedErrors", "true")
                .UseIISIntegration()
                .UseStartup<Startup>();
    }
}
