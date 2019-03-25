using EpMon.Monitor;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EpMon.Web.Core
{

    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            JobManager.Initialize(new MonitorRegistry());
        }
        private void OnShutdown()
        {
            JobManager.StopAndBlock();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime/*, ILoggerFactory loggerFactory*/)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
           
            //loggerFactory.AddLog4Net();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }
    }
}
