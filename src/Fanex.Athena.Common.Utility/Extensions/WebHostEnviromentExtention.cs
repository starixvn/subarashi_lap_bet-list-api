using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Fanex.Athena.Common.Utility.Extensions
{
    public static class WebHostEnviromentExtention
    {
        public static string MapPath(this IHostEnvironment hostEnvironment, string path)
            => Path.Combine(hostEnvironment.ContentRootPath, path);
    }
}
