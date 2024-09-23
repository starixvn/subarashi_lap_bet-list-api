using Fanex.Athena.Common.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


namespace Fanex.Athena.Common.Site.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ApiBetlistAuthorizeAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public ApiBetlistAuthorizeAttribute()
        {
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var licenseeSettingService = context.HttpContext.RequestServices.GetService<ILicenseeSettingService>();
            var contextService = context.HttpContext.RequestServices.GetService<IContextService>();

            var lincenseeName = contextService.SiteName;
            if (!licenseeSettingService.IsAuthorizedOnBetList(lincenseeName))
            {
                UnauthorizeAccessHandlerAsync(context);
                return;
            }
        }

        private void UnauthorizeAccessHandlerAsync(AuthorizationFilterContext actionContext)
        {
            actionContext.Result = new JsonResult(new
            {
                Error = "Unauthorized access"
            })
            {
                StatusCode = 401
            };
        }
    }
}
