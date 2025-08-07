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

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        public async Task GetRunningBetListAsync_WhenCallWithoutSort_ParameterIsPassesCorrectly(int sortType, int sortOrder)
        {
            // Arrange
            Reader.Read().Returns(false);
            Reader.NextResult().Returns(false);

            var metaModel = new PlayerAdminBetListMetaModel { SortType = sortType, SortOrder = sortOrder };

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

            var metaModel = new PlayerAdminBetListMetaModel { SortType = 1, SortOrder = 2 };

            // Action
            await DataAccess.GetRunningBetListAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("SortType", metaModel.SortType);
                ObjectDb.Received(1).SetParameterValue("SortOrder", metaModel.SortOrder);
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
