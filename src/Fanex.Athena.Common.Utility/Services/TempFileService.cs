using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Internal;
using Microsoft.Extensions.Hosting;

namespace Fanex.Athena.Common.Utility.Services
{
    public class TempFileService : ITempFileService
    {
        private readonly IAppSettings _appSettings;

        private readonly IFileSystem _fileSystem;

        private readonly IHostEnvironment _enviroment;

        private string RootPath => Path.IsPathFullyQualified(_appSettings.TempFilePath)
            ? _appSettings.TempFilePath
            : Path.Combine(_enviroment.ContentRootPath, _appSettings.TempFilePath);

        public TempFileService(IAppSettings appSetting, IFileSystem fileSystem, IHostEnvironment environment)
        {
            _appSettings = appSetting ?? new AppSettings();
            _fileSystem = fileSystem ?? new FileSystem();
            _enviroment = environment;
        }

        public void Delete(string path)
        {

            try
            {
                var realPath = Path.Combine(RootPath, path);

                _fileSystem.Delete(realPath);
            }
            catch
            {
                // Ignore it
            }
        }

        public string ReadString(string path)
        {
            var realPath = Path.Combine(RootPath, path);
            return _fileSystem.ReadString(realPath);
        }

        public void Save(string path, string content)
        {
            Directory.CreateDirectory(RootPath);

            var realPath = Path.Combine(RootPath, path);
            _fileSystem.Save(realPath, content);
        }

        public byte[] ReadBytes(string path)
        {
            var realPath = Path.Combine(RootPath, path);
            return _fileSystem.ReadAllBytes(realPath);
        }
    }
}