using Microsoft.AspNetCore.Authorization;

namespace Fanex.Athena.Common.Site.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class BearerAuthorizeAttribute : AuthorizeAttribute
    {
        public BearerAuthorizeAttribute(params string[] roles)
        {
            if (roles.Any())
            {
                Roles = string.Join(",", roles);
            }
            AuthenticationSchemes = "Bearer";
        }
    }
}
