﻿using System;
using System.Net;
using System.Threading;
using EpMon.Data;
using EpMon;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace EpMon.Web.Core
{

    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            
            ServicePointManager.DefaultConnectionLimit = 1000;
            ThreadPool.SetMinThreads(100, 100);
        }

        private void OnShutdown()
        {
            JobManager.StopAndBlock();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EpMonContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);
            
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            services.AddSingleton<HttpClientFactory, HttpClientFactory>();
            services.AddTransient<EndpointMonitor, EndpointMonitor>();
            services.AddTransient<EndpointStore, EndpointStore>();
            services.AddTransient<EndpointService, EndpointService>();

            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "EpMon API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
           
            loggerFactory.AddLog4Net();

            MigrateDatabase(app.ApplicationServices);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EpMon API V1");
            });

            
            JobManager.UseUtcTime();
            JobManager.Initialize(new MonitorOrchestrator(app.ApplicationServices));
            JobManager.Initialize(new CleanupOrchestrator(app.ApplicationServices));

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void MigrateDatabase(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<EpMonContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
