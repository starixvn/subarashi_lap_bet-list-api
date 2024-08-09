using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Internal;
using RazorEngine.Templating;

namespace Fanex.Athena.Common.Utility.Services
{
    public class TemplateBuilder : ITemplateBuilder
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRazorEngineService _razorEngine;

        public TemplateBuilder(IFileSystem fileSystem, IRazorEngineService razorEngine)
        {
            _fileSystem = fileSystem;
            _razorEngine = razorEngine;
        }

        public string BuildContent<T>(string path, T model)
        {
            var templateContent = _fileSystem.ReadString(path).Trim();
            var template = _razorEngine.RunCompile(templateContent, path, typeof(T), model: model);
            return template;
        }
    }


}