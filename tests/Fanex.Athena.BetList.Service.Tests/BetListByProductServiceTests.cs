using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.TestHelper;
using NSubstitute;

namespace Fanex.Athena.BetList.Service.Tests
{
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
