namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    using BetListNPOI.SS.UserModel;
    using Common.Constant;
    using Common.Utility.Collections;
    using Excel;
    using Excel.Model;
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using NUnit.Framework;
    using Resources;
    using Service.Interfaces;
    using TestClasses;
    using TestHelper;

    public class BettingTicketCenterBaseControllerTests : BaseControllerHelperTests
    {
        private ITicketBuilderService _ticketBuilderService;
        private IBettingTicketCenterExcel _bettingTicketCenterExcel;

        [SetUp]
        public void FixtureSetUp()
        {
            _ticketBuilderService = Substitute.For<ITicketBuilderService>();
            _bettingTicketCenterExcel = Substitute.For<IBettingTicketCenterExcel>();
        }

        [TearDown]
        public void FixtureTearDown()
        {
            _bettingTicketCenterExcel.Dispose();
            _ticketBuilderService = null;
            _bettingTicketCenterExcel = null;
        }

        [Test]
        public void ExportExcel_WhenCall_ExportCorrectData()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BettingTicketCenterExcelModel>(x => x.Data, null);

            var ticketResult = new TicketResult
            {
                MainTicketList = new SortableList<LrfTicket>
                    {
                        FakeHelper.Create<LrfTicket>()
                    }
            };

            metaModel.Data = ticketResult;

            var workbook = Substitute.For<IWorkbook>();
            _bettingTicketCenterExcel.Export(Arg.Any<BettingTicketCenterExcelModel>()).Returns(workbook);

            // Action
            var result = controller.ExportExcel(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(metaModel.Data, Is.Null);
            });
        }

        [Test]
        public void ReturnData_ShowTotal_GenerateCorrectData()
        {
            var controller = CreateController();

            // Assert
            var option = FakeHelper.Create<BettingTicketCenterOption>();
            var metaModel = Substitute.For<ICommonValueMetaModel>();

            option.IsBookedBet = true;
            option.ShowTotal = true;
            option.ShowWinLoss = true;
            option.ShowLicenseeWl = true;
            option.ShowSumStake = true;

            var ticketResult = new TicketResult
            {
                TotalRows = FakeHelper.Create<int>(),
                SumLicenseeWL = FakeHelper.Create<decimal>(),
                SumComm = FakeHelper.Create<decimal>(),
                SumStake = FakeHelper.Create<decimal>()
            };

            // Action
            var result = controller.ReturnData(ticketResult, option, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.GetValue("Total").GetValue("WinLoss"), Is.Not.Null);
                Assert.That(result.GetValue("Total").GetValue("LicenseeWL"), Is.EqualTo((ticketResult.SumLicenseeWL + ticketResult.SumComm).ToString(FormatKey.DecimalFormat)));
                Assert.That(result.GetValue("TotalRows"), Is.EqualTo(ticketResult.TotalRows));
                Assert.That(result.GetValue("Total").GetValue("Stake"), Is.EqualTo((ticketResult.SumStake).ToString(FormatKey.DecimalFormat)));
                _ticketBuilderService.Received().BuildBookedBetTicketText(ticketResult, metaModel);
            });

        }

        [Test]
        public void ReturnData_DoNotShowTotal_GenerateCorrectData()
        {
            // Assert
            var controller = CreateController();
            var ticketResult = new TicketResult();
            var metaModel = Substitute.For<ICommonValueMetaModel>();

            // Action
            var result = controller.ReturnData(ticketResult, new BettingTicketCenterOption(), metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.GetValue("Total").GetValue("Stake"), Is.Null);
                _ticketBuilderService.Received().BuildTicketText(ticketResult, metaModel, false);
            });
        }

        [Test]
        public void GetTitle_WhenViewSabaCoins_ReturnCorrectTitle()
        {
            //Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BaseBettingTicketMetaModel>(x => x.ViewSabaCoins, true);

            //Action
            var result = controller.GetTitle(metaModel);

            //Assert
            Assert.That(result, Is.EqualTo($"{ViewResources.BettingTicketCenter} {string.Format(ViewResources.Viewin, ViewResources.SabaCoins)}"));
        }

        [Test]
        public void GetTitle_WhenFromDateIsMinValue_ReturnCorrectTitle()
        {
            //Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BaseBettingTicketMetaModel>(x => x.ViewSabaCoins, false);
            metaModel.FromDate = DateTimeOffset.MinValue.DateTime;
            metaModel.IsConvertAll = false;

            //Action
            var result = controller.GetTitle(metaModel);

            //Assert
            Assert.That(result, Is.EqualTo(ViewResources.BettingTicketCenter));
        }

        [Test]
        public void GetTitle_WhenFromDateIsNotMinValue_ReturnCorrectTitle()
        {
            //Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BaseBettingTicketMetaModel>(x => x.ViewSabaCoins, false);
            metaModel.FromDate = DateTimeOffset.Now.DateTime;
            metaModel.IsConvertAll = true;
            metaModel.BaseCurrencyName = "USD";
            metaModel.BaseCurrency = "USD";

            //Action
            var result = controller.GetTitle(metaModel);

            //Assert
            Assert.That(result, Is.EqualTo($"{ViewResources.BettingTicketCenter}, {metaModel.FromDate.ToString(FormatKey.DateFormat)}-->{metaModel.ToDate.ToString(FormatKey.DateFormat)}, {ViewResources.BaseCurrency}: {metaModel.BaseCurrency.ToUpperInvariant()}"));
        }

        private BettingTicketCenterBaseControllerTestClass CreateController()
            => new(_ticketBuilderService, _bettingTicketCenterExcel);
    }
}
