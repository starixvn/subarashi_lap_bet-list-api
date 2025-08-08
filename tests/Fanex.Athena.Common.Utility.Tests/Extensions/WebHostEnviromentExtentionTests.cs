namespace Fanex.Athena.Common.Utility.Tests.Extensions
{
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Utility.Extensions;

    public class WebHostEnviromentExtentionTests
    {
        [Test]
        public void MapPath_Should_Return_Correct_Path()
        {
            // Arrange
            var hostEnvironment = new FakeHostEnvironment
            {
                ContentRootPath = "C:\\Projects\\MyApp"
            };
            var path = "Data\\file.txt";

            // Act
            var result = hostEnvironment.MapPath(path);

            // Assert
            Assert.That(result, Is.EqualTo(Path.Combine(hostEnvironment.ContentRootPath, path)));
        }

        // Fake implementation of IHostEnvironment for testing purposes
        private class FakeHostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; }
            public string ApplicationName { get; set; }
            public string ContentRootPath { get; set; }
            public IFileProvider ContentRootFileProvider { get; set; }
        }
    }
}
