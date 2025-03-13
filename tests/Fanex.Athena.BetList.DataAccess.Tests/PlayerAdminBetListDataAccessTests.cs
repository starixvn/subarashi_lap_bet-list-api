using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.TestHelper;
using Fanex.Athena.TestHelper.Extensions;
using Fanex.BetList.Core.Entities;
using NSubstitute;
using System.Collections;

namespace Fanex.Athena.BetList.DataAccess.Tests
{
    public class PlayerAdminBetListDataAccessTests : BaseDataAccessHelperTest<PlayerAdminBetListDataAccess>
    {
        protected override void SetUpDataAccess()
        {
            DataAccess = Substitute.ForPartsOf<PlayerAdminBetListDataAccess>(
                CurrencyDataAccess);
        }

        [TestCase(null, null)]
        [TestCase("", "ASC")]
        [TestCase("Index", "")]
        public async Task GetRunningBetListAsync_WhenCallWithoutSort_ParameterIsPassesCorrectly(string sortBy, string sortOrder)
        {
            // Arrange
            Reader.Read().Returns(false);
            Reader.NextResult().Returns(false);

            var metaModel = new PlayerAdminBetListMetaModel { SortBy = sortBy, SortOrder = sortOrder };

            // Action
            await DataAccess.GetRunningBetListAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("CustId", metaModel.CustId);
                ObjectDb.Received(1).SetParameterValue("PageIndex", metaModel.PageIndex);
                ObjectDb.Received(1).SetParameterValue("PageSize", metaModel.PageSize);
                ObjectDb.Received(1).SetParameterValue("SumStake", 0);
                ObjectDb.Received(1).SetParameterValue("TotalRows", 0);
            });
        }

        [Test]
        public async Task GetRunningBetListAsync_WhenSort_ParameterIsPassesCorrectly()
        {
            // Arrange
            Reader.Read().Returns(false);
            Reader.NextResult().Returns(false);

            var metaModel = new PlayerAdminBetListMetaModel { SortBy = FakeHelper.Create<string>(), SortOrder = FakeHelper.Create<string>() };

            // Action
            await DataAccess.GetRunningBetListAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("Sort", $"{metaModel.SortBy} {metaModel.SortOrder}");
            });
        }

        [Test]
        public async Task GetRunningBetListAsync_WhenCall_ReadReferenceTicket()
        {
            // Arrange
            var metaModel = new PlayerAdminBetListMetaModel();

            Reader.Read().Returns(false, true, false);
            Reader.NextResult().Returns(true);

            var expectResult = new TicketData
            {
                RefNo = FakeHelper.Create<string>(),
                Status = FakeHelper.Create<string>(),
                BetTypeId = FakeHelper.Create<int>(),
                Odds = FakeHelper.Create<int>()
            };
            MapReaderData(new
            {
                refno = expectResult.RefNo,
                status = expectResult.Status,
                BetType = expectResult.BetTypeId,
                odds = expectResult.Odds
            });

            // Action
            var result = await DataAccess.GetRunningBetListAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.TicketDataList[0], Tobe.SameProperties(expectResult));
            });
        }

        [Test]
        public async Task GetRunningBetListAsync_WhenCall_ReturnCorrectData()
        {
            // Arrange
            var metaModel = new PlayerAdminBetListMetaModel();
            var sumStake = FakeHelper.Create<decimal>();
            var totalRows = FakeHelper.Create<int>();

            ObjectDb.GetOutputParams().Returns(new Hashtable { { "SumStake", sumStake }, { "TotalRows", totalRows } });
            Reader.Read().Returns(false);
            Reader.NextResult().Returns(false);

            // Action
            var result = await DataAccess.GetRunningBetListAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalRows, Is.EqualTo(totalRows));
                Assert.That(result.SumStake, Is.EqualTo(sumStake));
            });
        }
    }
}
