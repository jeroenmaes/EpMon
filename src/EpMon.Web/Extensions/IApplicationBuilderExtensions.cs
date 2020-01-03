using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace EpMon.Web.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseOpenApiUi(this IApplicationBuilder app, IConfiguration configuration)
        {
            var virtualDirectory = configuration.GetSection("EpMon:SwaggerVirtualPath").Value;

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{virtualDirectory}/swagger/v1/swagger.json", "EpMon API V1");
            });

            return app;
        }
    }
}
