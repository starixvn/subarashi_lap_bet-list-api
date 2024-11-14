

using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using NSubstitute;

namespace Fanex.Athena.BetList.DataAccess.Tests
{
    public class BetListByProductDataAccessTests
    {
        private IBalanceUpDataAccess _balanceUpDataAccess;
        private ICurrencyDataAccess _currencyDataAccess;
        private IBetListByProductDataAccess _dataAccess;

        [SetUp]
        public void Setup()
        {
            _balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
            _currencyDataAccess = Substitute.For<ICurrencyDataAccess>();
            _dataAccess = new BetListByProductDataAccess(_balanceUpDataAccess, _currencyDataAccess);
        }

        //[Test]
        //public async Task QueryAsync_ShouldReturnTicketResult()
        //{
        //    // Arrange
        //    var metaModel = new BetListByProductMetaModel();
        //    var expectedTicketResult = new TicketResult();

        //    _balanceUpDataAccess.GetBalanceUp().Returns(new BalanceUp());
        //    _currencyDataAccess.GetCurrencyListAsync().Returns());

        //    // Act
        //    var result = await _dataAccess.QueryAsync(metaModel);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(expectedTicketResult.GetType(), result.GetType());
        //}
    }
}
