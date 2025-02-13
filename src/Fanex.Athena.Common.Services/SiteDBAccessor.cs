using Fanex.Athena.Common.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Fanex.Athena.Common.Services
{
    public class SiteDBAccessor : ISiteDBAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected HttpContext Context => _httpContextAccessor.HttpContext;

        public SiteDBAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string SiteDBServer => !string.IsNullOrWhiteSpace(GetClaimValue("SiteDBServer"))
            ? GetClaimValue("SiteDBServer")
            : GetSiteDBFromHeader();


        public bool ForceIgnoreSharding => bool.TrueString.Equals(Context.Request.Headers["X-IgnoreSharding"], StringComparison.OrdinalIgnoreCase);

        private string GetSiteDBFromHeader()
            => !string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request?.Headers["X-SiteDBServer"])
            ? _httpContextAccessor.HttpContext.Request.Headers["X-SiteDBServer"]
            : string.Empty;

        private string GetClaimValue(string claimType)
            => Context.User.FindFirst(claimType)?.Value;
    }
}
