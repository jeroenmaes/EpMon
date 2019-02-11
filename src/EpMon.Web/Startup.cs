using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using EpMon.Monitor;
using EpMon.Web;
using FluentScheduler;
using Microsoft.Owin;
using Microsoft.Owin.Security.ApiKey;
using Microsoft.Owin.Security.ApiKey.Contexts;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace EpMon.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseApiKeyAuthentication(new ApiKeyAuthenticationOptions
            {
                Provider = new ApiKeyAuthenticationProvider
                {
                    OnValidateIdentity = ValidateIdentity
                },
                Header = "X-API-KEY",
                HeaderKey = string.Empty
            });

            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);
        }
        
        private static Task ValidateIdentity(ApiKeyValidateIdentityContext context)
        {
            if (context.ApiKey == "1234")
            {
                context.Validate();
            }

            return Task.FromResult(0);
        }
    }
}