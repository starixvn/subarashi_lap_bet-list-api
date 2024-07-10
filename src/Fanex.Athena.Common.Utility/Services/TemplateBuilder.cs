using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Internal;
using RazorEngineCore;
//using RazorEngine;

namespace Fanex.Athena.Common.Utility.Services
{
    public class TemplateBuilder : ITemplateBuilder
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRazorEngine _razorEngine;

        public TemplateBuilder(IFileSystem fileSystem, IRazorEngine razorEngine)
        {
            _fileSystem = fileSystem;
            _razorEngine = razorEngine;
        }

        public string BuildContent(string path, object model)
        {
            var templateContent = _fileSystem.ReadString(path);
            var template =  _razorEngine.Compile(templateContent);
            return template.Run(model);

            //return Engine.Razor.RunCompile(templateContent, path, model: model);
        }
    }
}