using BetListNPOI.SS.UserModel;
using Fanex.Athena.BetList.Controllers.Utility;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.TestHelper.Extensions;
using NSubstitute;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;


namespace Fanex.Athena.BetList.Controllers.UnitTests.Utitlity
{
    public class BettingTicketCenterZipBuilderTests
    {
        private IAppSettings _appSettings;

        private ITempFileService _tempFileService;

        private BettingTicketCenterZipBuilder _zipBuilder;

        private IBettingTicketCenterExcel _bettingTicketCenterExcel;

        private IIPService _ipService;

        [SetUp]
        public void SetUp()
        {
            _appSettings = Substitute.For<IAppSettings>();
            _tempFileService = Substitute.For<ITempFileService>();
            _bettingTicketCenterExcel = Substitute.For<IBettingTicketCenterExcel>();
            _ipService = Substitute.For<IIPService>();

            _zipBuilder = Substitute.ForPartsOf<BettingTicketCenterZipBuilder>(_appSettings,
                _tempFileService, _bettingTicketCenterExcel, _ipService);
            _zipBuilder.When(x => x.InvokeProtectedMethod("SaveFile", Arg.Any<IWorkbook>(), Arg.Any<string>())).DoNotCallBase();
        }

        [TearDown]
        public void TearDown()
        {
            _bettingTicketCenterExcel.Dispose();
            _appSettings = null;
            _tempFileService = null;
            _bettingTicketCenterExcel = null;
        }

        [Test]
        public void BuildZip_HaveData_SaveFile()
        {
            // Arrange
            const string DataPath = "abc.json";

            var ticketResult = new GroupTicketResult
            {
                DataPath = new List<string> { DataPath }
            };

            _tempFileService.ReadString(DataPath).Returns("{\"MainTicketList\":[{\"ExternalId\":\"ThienPham2\"}],\"TicketDataList\":[]}");
            var workbook = Substitute.For<IWorkbook>();
            _bettingTicketCenterExcel.Export(Arg.Any<BettingTicketCenterExcelModel>()).Returns(workbook);
            var model = new AdvancedSearchBetListMetaModel();

            // Action
            var result = _zipBuilder.BuildZip(ticketResult, null, model, null);

            // Assert
            Assert.Multiple(() =>
            {
                _bettingTicketCenterExcel.Received(1).Export(Arg.Is<BettingTicketCenterExcelModel>(
                    x => x.Option.ShowTotal
                    && x.Option.ShowSumStake
                    && x.Option.ShowLicenseeWl
                    && x.Option.ShowWinLoss
                    && x.Option.ShowExternalId
                    && x.Option.ShowSystemId));
                _zipBuilder.Received(1).InvokeProtectedMethod("SaveFile", workbook, DataPath + ".dat");
                Assert.That(result, Is.Not.Null);
            });
        }
    }
}
