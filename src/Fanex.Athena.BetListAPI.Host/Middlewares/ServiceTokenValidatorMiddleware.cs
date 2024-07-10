using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Site.Authentications;
using System.Security.Claims;

namespace Fanex.Athena.BetListAPI.Site.Middlewares
{
    public class ServiceTokenValidatorMiddleware
    {
        private const string TOKEN_BETLIST_HEADER_NAME = "BETLIST_TOKEN_KEY";

        private readonly RequestDelegate _next;
        private readonly ILogger<ServiceTokenValidatorMiddleware> _logger;

        public ServiceTokenValidatorMiddleware(RequestDelegate next, ILogger<ServiceTokenValidatorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers[TOKEN_BETLIST_HEADER_NAME].FirstOrDefault();
            var configuration = context.RequestServices.GetService<IConfiguration>();
            var clients = configuration.GetSection("ServiceClients").Get<List<ServiceClient>>();
            if (!string.IsNullOrWhiteSpace(token) && clients.Any(c => c.Secret == token))
            {
                context.User?.Claims.Append(new Claim(Claims.APIToken, token));
                context.User?.Claims.Append(new Claim(Claims.IsValidClient, true.ToString()));
            }    

            await _next.Invoke(context);
        }
    }
}
