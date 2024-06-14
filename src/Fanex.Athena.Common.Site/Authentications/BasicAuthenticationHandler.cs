using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Fanex.Athena.Common.Site.Authentications
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly ILogger<BasicAuthenticationHandler> _logger;

        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger<BasicAuthenticationHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var crediential = Options.CredentialRetreival(Request);
            if (crediential == null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            var client = Options.Clients.FirstOrDefault(c => c.Name == crediential.Name && c.Secret == crediential.Secret);

            if (client == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));
            }
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, client.Name) }, Scheme.Name)), Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
