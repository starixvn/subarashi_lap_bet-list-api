namespace Fanex.Athena.Common.Utility.Tests.Helpers
{
    using Data;
    using Microsoft.AspNetCore.Http;
    using TestHelper;
    using Utility.Helpers;

    public class LogMessageHelperTests
    {
        [Test]
        public void GenerateMessage_Should_Return_Correct_Message()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new Exception("Test Exception");

            // Act
            var result = LogMessageHelper.GenerateMessage(context, exception);

            // Assert
            Assert.That(result, Contains.Substring("ErrMsg: Test Exception"));
            Assert.That(result, Contains.Substring("REQUEST INFO"));
            Assert.That(result, Contains.Substring("DATABASE INFO"));
            Assert.That(result, Contains.Substring("EXCEPTION INFO"));
            Assert.That(result, Contains.Substring("Message: Test Exception"));
            Assert.That(result, Contains.Substring("Source:"));
            Assert.That(result, Contains.Substring("TargetSite:"));
            Assert.That(result, Contains.Substring("StackTrace:"));
        }

        [Test]
        public void GenerateMessage_Should_Return_Correct_Message_With_Inner_Exception()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var innerException = new Exception("Inner Exception");
            var exception = new Exception("Test Exception", innerException);

            // Act
            var result = LogMessageHelper.GenerateMessage(context, exception);

            // Assert
            Assert.That(result, Contains.Substring("ErrMsg: Test Exception"));
            Assert.That(result, Contains.Substring("REQUEST INFO"));
            Assert.That(result, Contains.Substring("DATABASE INFO"));
            Assert.That(result, Contains.Substring("EXCEPTION INFO"));
            Assert.That(result, Contains.Substring("Message: Test Exception"));
            Assert.That(result, Contains.Substring("Source:"));
            Assert.That(result, Contains.Substring("TargetSite:"));
            Assert.That(result, Contains.Substring("StackTrace:"));
            Assert.That(result, Contains.Substring("Inner Exception"));
        }

        [Test]
        public void GenerateMessage_Should_Return_Correct_Message_With_Db_Exception()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var sqlException = DBTestHelper.MakeSqlException();

            var dbException = new ObjectDbException(sqlException,
                "Test Setting Key",
                "Test SpName",
                "Test DbName",
                "Data Source=localhost;Initial Catalog=GUARENTEE_FALSE;integrated security=false; User Id=user;Password=password;TrustServerCertificate=True;",
                1, new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } });

            // Act
            var result = LogMessageHelper.GenerateMessage(context, dbException);

            // Assert
            Assert.That(result, Contains.Substring("REQUEST INFO"));
            Assert.That(result, Contains.Substring("DATABASE INFO"));
            Assert.That(result, Contains.Substring("EXCEPTION INFO"));
            Assert.That(result, Contains.Substring("Source:"));
            Assert.That(result, Contains.Substring("TargetSite:"));
            Assert.That(result, Contains.Substring("StackTrace:"));
            Assert.That(result, Contains.Substring("Server:"));
            Assert.That(result, Contains.Substring("DbName:"));
            Assert.That(result, Contains.Substring("SpName:"));
            Assert.That(result, Contains.Substring("Parameters:"));
            Assert.That(result, Contains.Substring("CommandTimeout:"));
        }
    }
}
