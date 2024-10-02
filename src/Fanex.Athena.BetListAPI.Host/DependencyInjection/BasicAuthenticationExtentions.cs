using Fanex.Athena.Common.Site.Authentications;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Fanex.Athena.BetListAPI.Host.DependencyInjection
{
    public static class BasicAuthenticationExtentions
    {
        private const string Scheme = "Basic";

        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, Action<BasicAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddSingleton<IPostConfigureOptions<BasicAuthenticationOptions>, PostConfigureBasicAuthenticationOption>();
            return builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Scheme, configureOptions);
        }
    }
}
