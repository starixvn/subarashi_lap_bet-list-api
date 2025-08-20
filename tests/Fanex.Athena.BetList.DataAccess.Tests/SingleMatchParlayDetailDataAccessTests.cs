namespace Fanex.Athena.BetList.DataAccess.Tests
{
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using TestHelper;

    public class SingleMatchParlayDetailDataAccessTests : BaseDataAccessHelperTest<SingleMatchParlayDetailDataAccess>
    {
        [Test]
        public async Task GetSingleMatchParlayDetail_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            var metaModel = FakeHelper.Create<SingleMatchParlayDetailMetaModel>();

            Reader.Read().Returns(false);

            // Action
            await DataAccess.GetSingleMatchParlayDetail(metaModel).ConfigureAwait(false);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("TransId", metaModel.TransId);
                ObjectDb.Received(1).SetParameterValue("CustId", metaModel.CustId);
            });
        }

        [Test]
        public async Task GetSingleMatchParlayDetail_WhenCall_ReturnCorrectData()
        {
            // Arrange
            var metaModel = FakeHelper.Create<SingleMatchParlayDetailMetaModel>();

            Reader.Read().Returns(true, false);

            var expectedResutl = FakeHelper.Create<SingleMatchParlayDetail>();
            MapReaderData(expectedResutl);

            // Action
            var result = await DataAccess.GetSingleMatchParlayDetail(metaModel).ConfigureAwait(false);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result[0].SelectionType, Is.EqualTo(expectedResutl.SelectionType));
                Assert.That(result[0].Result, Is.EqualTo(expectedResutl.Result));
            });
        }
    }
}
