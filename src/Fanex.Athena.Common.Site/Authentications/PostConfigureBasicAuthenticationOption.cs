using Microsoft.Extensions.Options;

namespace Fanex.Athena.Common.Site.Authentications
{
    public class PostConfigureBasicAuthenticationOption : IPostConfigureOptions<BasicAuthenticationOptions>
    {
        public void PostConfigure(string name, BasicAuthenticationOptions options)
        {
            if (options.Clients == null)
            {
                options.Clients = Enumerable.Empty<ServiceClient>();
            }
        }
    }
}
