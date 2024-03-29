﻿using EpMon.Data;
using EpMon.Infrastructure;
using EpMon.Web.Extensions;
using EpMon.Web.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using EpMon.Publisher;
using Microsoft.Extensions.Hosting;
using EpMon.Web.Api;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace EpMon.Web
{

    public class Startup
    {
        private MetricPusher _metricPusher;

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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {           
            services.AddLogging(
                    c =>
                    {
                        c.AddConsole();
                        c.AddFile(Configuration.GetSection("Logging"));
                    });

            services.AddDbContext<EpMonContext>();
            services.AddStartupJob<MigrateDatabaseJob>();

            services.AddHttpClient("default").ConfigurePrimaryHttpMessageHandler(messageHandler =>
            {
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                }
                //do not use cookies to avoid load balancer issues
                handler.UseCookies = false;
                return handler;
            });

            services.AddSingleton<ITokenService, CachedTokenService>();
            services.AddTransient<EndpointMonitor, EndpointMonitor>();
            services.AddTransient<EndpointStore, EndpointStore>();
            services.AddTransient<EndpointService, EndpointService>();
            services.AddSingleton<PrometheusPublisher, PrometheusPublisher>();

            var aiKey = Configuration.GetSection("EpMon:ApplicationInsightsKey").Value;
            var publisher = new ApplicationInsightsPublisher(aiKey);
            services.AddSingleton(publisher);

            services.AddMvc(options => options.EnableEndpointRouting = false).AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "EpMon API", Version = "v1" });

                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "ApiKey must appear in header",
                    Type = SecuritySchemeType.ApiKey,
                    Name = "X-API-Key",
                    In = ParameterLocation.Header,
                    Scheme = "ApiKeyScheme"
                });
                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() }
                    };
                c.AddSecurityRequirement(requirement);
            });

            services.AddScheduledJobs();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment env)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseOpenApiUi(Configuration);

            Metrics.SuppressDefaultMetrics();
            var prometheusEndpoint = Configuration.GetSection("EpMon:PrometheusPushGateway").Value;
            if (!string.IsNullOrEmpty(prometheusEndpoint))
            {
                _metricPusher = new MetricPusher(prometheusEndpoint, $"EpMon", Environment.MachineName);
                _metricPusher.Start();
            }
        }

        private void OnShutdown()
        {
            _metricPusher?.Stop();
        }
    }
}
