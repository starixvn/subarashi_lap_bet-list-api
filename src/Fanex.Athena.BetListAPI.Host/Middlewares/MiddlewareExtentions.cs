using Fanex.Athena.BetListAPI.Host.Middlewares;
using Fanex.Athena.PlayerAdmin.Core.Middlewares;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Fanex.Athena.BetListAPI.Site.Middlewares
{
    public static class MiddlewareExtentions
    {
        public static IApplicationBuilder UsePing(this IApplicationBuilder app)
        {
            app.MapWhen(context => context.Request.Path.StartsWithSegments("/ping"), appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    await context.Response.WriteAsync("pong");
                });
            });
            return app;
        }

        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app)
        {
            app.MapWhen(context => context.Request.Path.StartsWithSegments("/whoami"), appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var healthCheckService = app.ApplicationServices.GetService<HealthCheckService>();
                    var result = await healthCheckService.CheckHealthAsync(predicate => predicate.Name.StartsWith("WhoAmI"));

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(result));
                });
            });
            app.MapWhen(context => context.Request.Path.StartsWithSegments("/health"), appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var healthCheckService = app.ApplicationServices.GetService<HealthCheckService>();
                    var result = await healthCheckService.CheckHealthAsync(predicate => !predicate.Name.StartsWith("WhoAmI"));

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(result));
                });
            });
            return app;
        }

        public static IApplicationBuilder UseDoubleSlashHandler(
            this IApplicationBuilder builder)
            => builder.UseMiddleware<DoubleSlashMiddleware>();

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ErrorHandlerMiddleware>();

        public static IApplicationBuilder UseServiceTokenValidator(this IApplicationBuilder builder)
            => builder.UseMiddleware<ServiceTokenValidatorMiddleware>();


    }
}
