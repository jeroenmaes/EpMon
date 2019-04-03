using EpMon.Data;
using EpMon.Monitor;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EpMon.Web.Core
{

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

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
            services.AddDbContext<EpMonContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("EpMonConnection"));
            });

            services.AddTransient<EpMonAsyncRepository, EpMonAsyncRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMiniProfiler().AddEntityFramework();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime/*, ILoggerFactory loggerFactory*/)
        {
#if DEBUG
            app.UseMiniProfiler();
#endif
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
