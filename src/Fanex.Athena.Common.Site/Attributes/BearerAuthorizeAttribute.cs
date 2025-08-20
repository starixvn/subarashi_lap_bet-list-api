namespace Fanex.Athena.Common.Site.Attributes
{
    using Microsoft.AspNetCore.Authorization;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class BearerAuthorizeAttribute : AuthorizeAttribute
    {
        public BearerAuthorizeAttribute(params string[] roles)
        {
            if (roles.Any())
            {
                Roles = string.Join(",", roles.Append(Constant.Roles.PortalSuperAdmin));
            }
            AuthenticationSchemes = "Bearer";
        }
    }
}
