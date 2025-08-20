namespace Fanex.Athena.Common.Service.UnitTests
{
    using System.Data;
    using Constant;
    using Data;
    using NSubstitute;
    using Services;
    using Services.Interfaces;
    using TestHelper;

    public class SubsidiaryServiceTests
    {
        private IContextService _contextService;
        private SubsidiaryService _subsidiaryService;
        private IObjectDb _objectDB;
        private IDataReader _dataReader;

        [SetUp]
        public void Setup()
        {
            _contextService = Substitute.For<IContextService>();
            _subsidiaryService = new SubsidiaryService(_contextService);
            _objectDB = Substitute.For<IObjectDb>();
            _dataReader = Substitute.For<IDataReader>();
            _objectDB.ExecuteReaderAsync().Returns(_dataReader);
            DBTestHelper.SetUpDBObjects(_objectDB);
        }

        [TearDown]
        public void TearDown()
        {
            _objectDB.Dispose();
            _dataReader.Dispose();
            _contextService = null;
            _subsidiaryService = null;
            _objectDB = null;
            _dataReader = null;
        }

        [Test]
        public void IsIgnoredSubsidiarySearch_ShouldReturnTrue_WhenSubsidiarySearchIsFalse()
        {
            // Arrange
            bool subsidiarySearch = false;
            string subsidiaryList = "1,2,3";

            // Act
            bool result = _subsidiaryService.IsIgnoredSubsidiarySearch(subsidiarySearch, subsidiaryList);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsIgnoredSubsidiarySearch_ShouldReturnTrue_WhenSubsidiaryListIsNullOrWhiteSpace()
        {
            // Arrange
            bool subsidiarySearch = true;
            string subsidiaryList = "";

            // Act
            bool result = _subsidiaryService.IsIgnoredSubsidiarySearch(subsidiarySearch, subsidiaryList);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsIgnoredSubsidiarySearch_ShouldReturnTrue_WhenSubsidiaryViewTypeIsNotManagementAndUserIsNotInRoles()
        {
            // Arrange
            bool subsidiarySearch = true;
            string subsidiaryList = "1,2,3";
            _contextService.SubsidiaryViewType.Returns(SubsidiaryViewType.NotManagement);
            _contextService.IsInRoles(Roles.Subsidiary).Returns(false);

            // Act
            bool result = _subsidiaryService.IsIgnoredSubsidiarySearch(subsidiarySearch, subsidiaryList);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsIgnoredSubsidiarySearch_ShouldReturnFalse_WhenAllConditionsAreMet()
        {
            // Arrange
            bool subsidiarySearch = true;
            string subsidiaryList = "1,2,3";
            _contextService.SubsidiaryViewType.Returns(SubsidiaryViewType.MasterManagement);
            _contextService.IsInRoles(Roles.Subsidiary).Returns(true);

            // Act
            bool result = _subsidiaryService.IsIgnoredSubsidiarySearch(subsidiarySearch, subsidiaryList);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetAgentList_ShouldReturnCorrectAgentList()
        {
            // Arrange
            string subsidiaryList = "sub1,sub2,sub3";
            var agentList = new Dictionary<int, int[]>
                    {
                        { 1, new[] { 1, 2, 3 } },
                        { 2, new[] { 4, 5, 6 } },
                        { 3, new[] { 7, 8, 9 } }
                    };
            _contextService.AgentList.Returns(agentList);
            _contextService.SiteName.Returns("SiteName");
            var mockReader = Substitute.For<IDataReader>();
            mockReader.Read().Returns(true, false);
            mockReader["AgentID"].Returns(1);
            mockReader["OperatorID"].Returns("sub1");
            mockReader["Currency"].Returns(1);
            _objectDB.ExecuteReaderAsync().Returns(mockReader);

            // Act
            var result = await _subsidiaryService.GetAgentList(subsidiaryList);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.ContainsKey(1), Is.True);
            Assert.That(result[1].Length, Is.EqualTo(1));
        }
    }
}
