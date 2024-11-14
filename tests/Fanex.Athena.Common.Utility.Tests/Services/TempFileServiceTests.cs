namespace Fanex.Athena.Common.Utility.UnitTests.Helpers
{
    using Fanex.Athena.Common.Utility.Services;
    using Fanex.Athena.Common.Utility.Services.Interfaces;
    using Fanex.Athena.Common.Utility.Services.Internal;
    using Microsoft.Extensions.Hosting;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TempFileServiceTest
    {
        private string TempFilePath => Directory.GetCurrentDirectory();

        private const string FilePath = "BTC/abc.json";

        private IAppSettings _appSettings;
        private IFileSystem _fileSystem;
        private IHostEnvironment _enviroment;
        private ITempFileService _tempFileService;

        [SetUp]
        public void SetUp()
        {
            _appSettings = Substitute.For<IAppSettings>();
            _fileSystem = Substitute.For<IFileSystem>();
            _enviroment = Substitute.For<IHostEnvironment>();

            _appSettings.TempFilePath.Returns(TempFilePath);

            _tempFileService = new TempFileService(_appSettings, _fileSystem, _enviroment);
        }

        [Test]
        public void Delete_WhenCall_CallFileSystemDelete()
        {
            // Action
            _tempFileService.Delete(FilePath);

            // Assert
            _fileSystem.Received(1).Delete(Path.Combine(TempFilePath, FilePath));
        }

        [Test]
        public void ReadString_WhenCall_ReturnDataFromFileSystem()
        {
            // Arrange
            const string ExpectedContent = "sdflsdlfksldf";
            _fileSystem.ReadString(Path.Combine(TempFilePath, FilePath)).Returns(ExpectedContent);

            // Action
            var content = _tempFileService.ReadString(FilePath);

            // Assert
            Assert.That(content, Is.EqualTo(ExpectedContent));
        }

        [Test]
        public void ReadAllBytes_WhenCall_CallFileSystemReadAllBytes()
        {
            // Arrange
            var expectedContent = new byte[] { 0x20, 0x21 };

            _fileSystem.ReadAllBytes(Path.Combine(TempFilePath, FilePath)).Returns(expectedContent);

            // Action
            _tempFileService.ReadBytes(FilePath);

            // Assert
            _fileSystem.Received(1).ReadAllBytes(Path.Combine(TempFilePath, FilePath));
        }

        [Test]
        public void Save_WhenCall_CreateDirectoryAndCallFileSystemSave()
        {
            // Arrange
            const string path = "BTC/abc.json";
            const string content = "sample content";

            // Action
            _tempFileService.Save(path, content);

            // Assert
            _fileSystem.Received(1).Save(Path.Combine(TempFilePath, path), content);
        }

        [Test]
        public void Save_WhenCallWithFullyQualifiedPath_CallFileSystemSave()
        {
            // Arrange
            var FullyQualifiedPath = Path.Combine(TempFilePath, FilePath);
            const string Content = "sample content";

            // Action
            _tempFileService.Save(FullyQualifiedPath, Content);

            // Assert
            _fileSystem.Received(1).Save(FullyQualifiedPath, Content);
        }
    }
}