using Fanex.Athena.Common.Utility.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Fanex.Athena.BetListAPI.Host.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

            if (error != null && error.Error != null)
            {
                HandleError(context, error.Error);
            }

            try
            {
                await _next.Invoke(context);
            }
            catch (Exception exception)
            {
                HandleError(context, exception);
            }
        }

        private void HandleError(HttpContext context, Exception exception)
        {
            var errorMsg = LogMessageHelper.GenerateMessage(context, exception);
            _logger.LogCritical(exception, errorMsg);
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
