using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Fanex.Athena.Common.Site.Authentications
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public IEnumerable<ServiceClient> Clients { get; set; }

        public Func<HttpRequest, ServiceClient> CredentialRetreival { get; set; } = request =>
        {
            var authorizationHeader = request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return null;
            }

            var credential = authorizationHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (credential.Length != 2 || !credential[0].Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var credentialBytes = Convert.FromBase64String(credential[1]);
            var credentialParts = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            if (credentialParts.Length != 2)
            {
                return null;
            }

            return new ServiceClient
            {
                Name = credentialParts[0],
                Secret = credentialParts[1]
            };
        };
    }
}
