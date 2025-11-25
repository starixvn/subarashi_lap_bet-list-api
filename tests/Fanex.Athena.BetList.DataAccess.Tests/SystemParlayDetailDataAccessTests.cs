namespace Fanex.Athena.BetList.DataAccess.Tests
{
    using System.Data;
    using Common.DataAccess.Interfaces;
    using Common.Model.DTO;
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using TestHelper;
    using TestHelper.Extensions;

    public class SystemParlayDetailDataAccessTests : BaseDataAccessHelperTest<SystemParlayDetailDataAccess>
    {
        private IBalanceUpDataAccess _balanceUpDataAccess;

        private readonly BalanceUp _balanceUpInfo = new BalanceUp
        {
            BalanceUpDay = new DateTime(2016, 8, 18),
            MoveBetsDay = new DateTime(2016, 8, 10)
        };

        protected override void SetUpDataAccess()
        {
            _balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
            _balanceUpDataAccess.GetBalanceUp().Returns(_balanceUpInfo);
            DataAccess = Substitute.ForPartsOf<SystemParlayDetailDataAccess>(_balanceUpDataAccess);
        }

        //[Test]
        //public async Task GetSystemParlayDetail_WhenCall_ParametersPassedCorrectly()
        //{
        //    // Arrange
        //    var metaModel = GetSystemParlayDetailMetaModel();

        //    var dataset = new DataSet();
        //    ObjectDb.ExecuteDataSetAsync().Returns(dataset);

        //    // Action
        //    await DataAccess.GetData(metaModel);

        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        ObjectDb.Received(1).SetParameterValue("refno", metaModel.RefNo);
        //        ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
        //        ObjectDb.Received(1).SetParameterValue("movebetsday", _balanceUpInfo.MoveBetsDay);
        //        ObjectDb.Received(1).SetParameterValue("balanceupday", _balanceUpInfo.BalanceUpDay);
        //        ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
        //        ObjectDb.Received(1).SetParameterValue("outstanding", null);
        //    });
        //}

        [Test]
        public async Task GetSystemParlayDetail_WhenCall_GetTicketDataCorrectly()
        {
            // Arrange
            var expectedResult = FakeHelper.Create<SystemParlayDetailTicket>();
            expectedResult.TransId = 0;
            var metaModel = GetSystemParlayDetailMetaModel();
            ObjectDb.ExecuteDataSetAsync().Returns(CreateFakeDataSetForSystemParlayTickeData(expectedResult));

            // Action
            var result = await DataAccess.GetData(metaModel).ConfigureAwait(false);

            // Assert
            Assert.That(result.TicketList.ElementAt(0), Tobe.SameProperties(expectedResult));
        }

        [Test]
        public async Task GetData_WhenCall_GetSerialDataCorrectly()
        {
            // Arrange
            var expectedResult = new SystemParlayDetailSerial
            {
                Serial = 1,
                HomeId = 2,
                AwayId = 3,
                IsNeutral = true
            };
            var metaModel = GetSystemParlayDetailMetaModel();
            ObjectDb.ExecuteDataSetAsync().Returns(CreateFakeDataSetForSystemParlaySerialData(expectedResult));

            // Action
            var result = await DataAccess.GetData(metaModel).ConfigureAwait(false);

            // Assert
            Assert.That(result.SerialList.ElementAt(0), Tobe.SameProperties(expectedResult));
        }

        private static DataSet CreateFakeDataSetForSystemParlayTickeData(SystemParlayDetailTicket data)
        {
            var dataSet = new DataSet();
            var table = new DataTable();
            dataSet.Tables.Add(table);
            table.Columns.Add("comm", typeof(decimal));
            table.Columns.Add("modds", typeof(decimal));
            table.Columns.Add("odds", typeof(decimal));
            table.Columns.Add("playercomm", typeof(decimal));
            table.Columns.Add("serial", typeof(long));
            table.Columns.Add("stake", typeof(decimal));
            table.Columns.Add("status", typeof(string));
            table.Columns.Add("statusid", typeof(int));
            table.Columns.Add("transid", typeof(long));
            table.Columns.Add("winlost", typeof(decimal));
            table.Columns.Add("winlostdate", typeof(DateTime));
            table.Columns.Add("BetTeam", typeof(string));

            table.Rows.Add(
                data.Comm,
                data.MOdds,
                data.Odds,
                data.PlayerComm,
                data.Serial,
                data.Stake,
                data.Status,
                data.StatusId,
                data.TransId,
                data.WinLost,
                data.WinLostDate,
                data.BetTeam ?? string.Empty);

            return dataSet;
        }

        private static DataSet CreateFakeDataSetForSystemParlaySerialData(SystemParlayDetailSerial data)
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            var table = new DataTable();
            dataSet.Tables.Add(table);
            table.Columns.Add("serial", typeof(long));
            table.Columns.Add("homeid", typeof(int));
            table.Columns.Add("awayid", typeof(int));
            table.Columns.Add("IsNeutral", typeof(bool));

            table.Rows.Add(data.Serial, data.HomeId, data.AwayId, data.IsNeutral);

            return dataSet;
        }

        private static SystemParlayDetailMetaModel GetSystemParlayDetailMetaModel(string baseCurrency = "")
        {
            return new SystemParlayDetailMetaModel
            {
                BaseCurrency = baseCurrency,
                RefNo = 1,
                UserId = 2,
                LicenseeId = 3
            };
        }
    }
}