namespace Fanex.Athena.Common.Site.Helpers
{
    using Microsoft.AspNetCore.Http;

    public static class RoutingHelper
    {
        public static PathString FixDoubleSlashes(this PathString path)
        {
            if (string.IsNullOrWhiteSpace(path.Value))
            {
                return path;
            }
            if (path.Value.Contains("//"))
            {
                return new PathString(path.Value.Replace("//", "/"));
            }
            return path;
        }
    }
}
