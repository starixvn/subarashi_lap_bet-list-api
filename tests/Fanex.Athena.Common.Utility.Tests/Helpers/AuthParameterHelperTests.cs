namespace Fanex.Athena.Common.Utility.Tests.Helpers
{
    using Constant;
    using Utility.Helpers;

    public class AuthParameterHelperTests
    {
        [Test]
        public void GenerateAgentListXml_WithCurrencyId_ReturnsXmlString()
        {
            // Arrange
            var currencyAgentList = new Dictionary<int, int[]>
                    {
                        { 1, new[] { 100, 200, 300 } },
                        { 2, new[] { 400, 500 } }
                    };
            int currencyId = 1;
            bool isExcludeTestData = false;

            // Act
            string result = AuthParameterHelper.GenerateAgentListXml(currencyAgentList, currencyId, isExcludeTestData);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("<ROOT>"));
            Assert.That(result, Contains.Substring("</ROOT>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"100\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"200\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"300\" Currency=\"1\"/>"));
            Assert.That(result, Does.Not.Contain("<Agent AgentId=\"400\" Currency=\"2\"/>"));
            Assert.That(result, Does.Not.Contain("<Agent AgentId=\"500\" Currency=\"2\"/>"));
        }

        [Test]
        public void GenerateAgentListXml_WithCurrencyIdList_ReturnsXmlString()
        {
            // Arrange
            var currencyAgentList = new Dictionary<int, int[]>
                    {
                        { 1, new[] { 100, 200, 300 } },
                        { 2, new[] { 400, 500 } }
                    };
            var currencyIdList = new List<int> { 1, 2 };
            bool isExcludeTestData = false;

            // Act
            string result = AuthParameterHelper.GenerateAgentListXml(currencyAgentList, currencyIdList, isExcludeTestData);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("<ROOT>"));
            Assert.That(result, Contains.Substring("</ROOT>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"100\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"200\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"300\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"400\" Currency=\"2\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"500\" Currency=\"2\"/>"));
        }

        [Test]
        public void GenerateAgentListXml_WithAllCurrencyId_ReturnsXmlString()
        {
            // Arrange
            var currencyAgentList = new Dictionary<int, int[]>
                    {
                        { 1, new[] { 100, 200, 300 } },
                        { 2, new[] { 400, 500 } }
                    };
            var currencyIdList = new List<int> { 0 };
            bool isExcludeTestData = false;

            // Act
            string result = AuthParameterHelper.GenerateAgentListXml(currencyAgentList, currencyIdList, isExcludeTestData);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("<ROOT>"));
            Assert.That(result, Contains.Substring("</ROOT>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"100\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"200\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"300\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"400\" Currency=\"2\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"500\" Currency=\"2\"/>"));
        }

        [Test]
        public void GenerateAgentListXml_WithExcludeTestData_ReturnsXmlString()
        {
            // Arrange
            var currencyAgentList = new Dictionary<int, int[]>
                    {
                        { 1, new[] { 100, 200, 300 } },
                        { CurrencyType.Uus, new[] { 400, 500 } }
                    };
            int currencyId = 1;
            bool isExcludeTestData = true;

            // Act
            string result = AuthParameterHelper.GenerateAgentListXml(currencyAgentList, currencyId, isExcludeTestData);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("<ROOT>"));
            Assert.That(result, Contains.Substring("</ROOT>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"100\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"200\" Currency=\"1\"/>"));
            Assert.That(result, Contains.Substring("<Agent AgentId=\"300\" Currency=\"1\"/>"));
            Assert.That(result, Does.Not.Contain("<Agent AgentId=\"400\" Currency=\"2\"/>"));
            Assert.That(result, Does.Not.Contain("<Agent AgentId=\"500\" Currency=\"2\"/>"));
        }
    }
}
