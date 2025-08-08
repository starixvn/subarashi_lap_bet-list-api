namespace Fanex.Athena.BetList.Service.Tests
{
    using DataAccess.Interfaces;
    using Interfaces;
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using TestHelper;

    public class BetListByProductServiceTests
    {
        private IBetListByProductDataAccess _betListByProductDataAccess;
        private IBetListByProductService _betListByProductService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _betListByProductDataAccess = Substitute.For<IBetListByProductDataAccess>();

            _betListByProductService = new BetListByProductService(
               _betListByProductDataAccess
                );
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _betListByProductDataAccess = null;
            _betListByProductService = null;
        }

        [Test]
        public async Task QueryAsync_WhenCall_CallDataAccess()
        {
            // Arrange
            var metaModel = FakeHelper.Create<BetListByProductMetaModel>();
            var data = new TicketResult();
            _betListByProductDataAccess.QueryAsync(metaModel).Returns(data);
            // Action
            var result = await _betListByProductService.QueryAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _betListByProductDataAccess.Received(1).QueryAsync(metaModel);
                result.Equals(data);
            });
        }

    }
}
