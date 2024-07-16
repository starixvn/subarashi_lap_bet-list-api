using Fanex.Athena.Common.Site.Helpers;

namespace Fanex.Athena.BetListAPI.Host.Middlewares
{
    public class DoubleSlashMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DoubleSlashMiddleware> _logger;

        public DoubleSlashMiddleware(RequestDelegate next, ILogger<DoubleSlashMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation($"Invoking {nameof(DoubleSlashMiddleware)} on {context.Request.Path}");

            context.Request.Path = context.Request.Path.FixDoubleSlashes();

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

}
