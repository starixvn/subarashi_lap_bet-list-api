namespace Fanex.Athena.Common.Utility.Extensions
{
    using Microsoft.Extensions.Hosting;

    public static class WebHostEnviromentExtention
    {
        public static string MapPath(this IHostEnvironment hostEnvironment, string path)
            => Path.Combine(hostEnvironment.ContentRootPath, path);
    }
}
