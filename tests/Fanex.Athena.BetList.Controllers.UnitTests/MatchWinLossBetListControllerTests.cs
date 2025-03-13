//using Fanex.Athena.BetList.Model.DTO;
//using Fanex.Athena.BetList.Model.MetaModel;
//using Fanex.Athena.BetList.Model.ViewModel;
//using Fanex.Athena.BetList.Service;
//using Fanex.Athena.BetList.Service.Interfaces;
//using Fanex.Athena.BetList.Service.Optiions;
//using Fanex.Athena.Common.DataAccess.Interfaces;
//using Fanex.Athena.Common.Model.DTO;
//using Fanex.Athena.Common.Services.Interfaces;
//using Fanex.Athena.Common.Utility.Collections;
//using Fanex.Athena.TestHelper;
//using Fanex.Core.Utils;
//using NSubstitute;
//using NUnit.Framework;

//namespace Fanex.Athena.BetList.Controllers.UnitTests
//{
//    public class MatchWinLossBetListControllerTests
//    {
//        private readonly string STR_SP_BETLIST_MATCHWL_OLD = "Lrf_BetList_MatchWL_Old";
//        private readonly string STR_SP_BETLIST_MATCHWL = "Lrf_BetList_MatchWL";

//        private IContextService _contextService;
//        private IAuthorizationService _authorizationService;
//        private IBetListService _betListService;
//        private ITicketBuilderService _ticketBuilderService;
//        private ISubsidiaryService _subsidiaryService;
//        private IBalanceUpDataAccess _balanceUpDataAccess;
//        private ICurrencyValidation _currencyValidation;

//        [SetUp]
//        public void SetUp()
//        {
//            _contextService = Substitute.For<IContextService>();
//            _authorizationService = Substitute.For<IAuthorizationService>();
//            _betListService = Substitute.For<IBetListService>();
//            _ticketBuilderService = Substitute.For<ITicketBuilderService>();
//            _subsidiaryService = Substitute.For<ISubsidiaryService>();
//            _balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
//            _currencyValidation = Substitute.For<ICurrencyValidation>();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _contextService = null;
//            _authorizationService = null;
//            _betListService = null;
//            _ticketBuilderService = null;
//            _subsidiaryService = null;
//            _balanceUpDataAccess = null;
//            _currencyValidation = null;
//        }

//        [TestCase(1, 20, 50, 3)]
//        public async Task BetTypeWinLoss_WhenCall_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int totalPage)
//        {
//            // Arrange
//            var controller = CreateController();
//            var metaModel = FakeHelper.Create<BetListMetaModel>();
//            var ticketResult = new TicketResult { TotalRows = totalRows };
//            _betListService.GetBetListData(metaModel).Returns(ticketResult);
//            _balanceUpDataAccess.GetBalanceUp().Returns(FakeHelper.Create<BalanceUp>());
//            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(true);

//            var ticketText = FakeHelper.CreateMany<TicketText>(2);
//            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, Arg.Any<int>(), Arg.Any<BuildTicketOption>()).Returns(ticketText.ToList());

//            // Action
//            var result = await controller.BetTypeWinLoss(pageIndex, pageSize ,metaModel);
//            var page = result.GetValue("page");
//            var total = result.GetValue("total");
//            var records = result.GetValue("records");
//            var rows = result.GetValue("rows");

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(page, Is.EqualTo(pageIndex));
//                Assert.That(total, Is.EqualTo(totalPage));
//                Assert.That(records, Is.EqualTo(totalRows));
//                Assert.That(rows, Is.EqualTo(ticketText));
//            });
//        }

//        [TestCase(1, 20, 50, 3)]
//        public async Task MatchBetType_WhenCall_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int totalPage)
//        {
//            // Arrange
//            var controller = CreateController();
//            var metaModel = FakeHelper.Create<BetListMetaModel>();
//            var ticketResult = new TicketResult { TotalRows = totalRows };
//            _betListService.GetBetListData(metaModel).Returns(ticketResult);
//            _balanceUpDataAccess.GetBalanceUp().Returns(FakeHelper.Create<BalanceUp>());
//            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(true);

//            var ticketText = FakeHelper.CreateMany<TicketText>(2);
//            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, Arg.Any<int>(), Arg.Any<BuildTicketOption>()).Returns(ticketText.ToList());

//            // Action
//            var result = await controller.MatchBetType(pageIndex, pageSize, metaModel);
//            var page = result.GetValue("page");
//            var total = result.GetValue("total");
//            var records = result.GetValue("records");
//            var rows = result.GetValue("rows");

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(page, Is.EqualTo(pageIndex));
//                Assert.That(total, Is.EqualTo(totalPage));
//                Assert.That(records, Is.EqualTo(totalRows));
//                Assert.That(rows, Is.EqualTo(ticketText));
//            });
//        }

//        [Test]
//        public async Task Outright_HasMainTicket_ReturnCorrectLeagueName()
//        {
//            var controller = CreateController();

//            // Arrange
//            var metaModel = FakeHelper.Create<BetListMetaModel>();
//            metaModel.IsServerPaging = false;

//            var ticket = FakeHelper.Create<LrfTicket>();
//            var ticketResult = new TicketResult
//            {
//                SumWinLoss = FakeHelper.Create<decimal>(),
//                SumPlayerComm = FakeHelper.Create<decimal>(),
//                SumLicenseeWL = FakeHelper.Create<decimal>(),
//                SumComm = FakeHelper.Create<decimal>(),
//                SumBetEffective = FakeHelper.Create<decimal>(),

//                MainTicketList = new SortableList<LrfTicket> { ticket }
//            };
//            _betListService.GetBetListData(metaModel).Returns(ticketResult);

//            var ticketText = FakeHelper.CreateMany<TicketText>(2).ToList();
//            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, Arg.Any<int>(), Arg.Any<BuildTicketOption>()).Returns(ticketText);

//            var leagueName = FakeHelper.Create<string>();
//            _betListService.GetLeagueName(ticket.LeagueId).Returns(leagueName);

//            _balanceUpDataAccess.GetBalanceUp().Returns(FakeHelper.Create<BalanceUp>());
//            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(true);

//            // Action
//            var result = await controller.Outright(metaModel);

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(result.Main, Is.EqualTo(ticketText));
//                Assert.That(result.Total.SumWinLoss, Is.EqualTo(ticketResult.SumWinLoss + ticketResult.SumPlayerComm));
//                Assert.That(result.Total.SumLicenseeWl, Is.EqualTo(ticketResult.SumLicenseeWL + ticketResult.SumComm));
//                Assert.That(result.Total.SumBetEffective, Is.EqualTo(ticketResult.SumBetEffective));
//                Assert.That(result.Total.SumStake, Is.EqualTo(Math.Round(ticket.Stake, 2)));
//                Assert.That(result.LeagueName, Is.EqualTo(leagueName));
//                Assert.That(metaModel.IsServerPaging, Is.True);
//            });

//        }

//        [TestCase(1, 20, 50, 3)]
//        public async Task MixParlay_WhenCallWithValidCurrency_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int totalPage)
//        {
//            // Arrange
//            var controller = CreateController();
//            var metaModel = FakeHelper.Create<MixParlayMetaModel>();
//            metaModel.Page = pageIndex;
//            metaModel.Rows = pageSize;
//            var ticketResult = new TicketResult { TotalRows = totalRows };
//            _betListService.GetBetListData((BetListMetaModel)default).ReturnsForAnyArgs(ticketResult);
//            _balanceUpDataAccess.GetBalanceUp().Returns(FakeHelper.Create<BalanceUp>());
//            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(true);

//            var ticketText = FakeHelper.CreateMany<TicketText>(2);
//            _ticketBuilderService.BuildTicketText(ticketResult, Arg.Any<BetListMetaModel>(), Arg.Any<int>(), Arg.Any<BuildTicketOption>()).Returns(ticketText.ToList());

//            _contextService.CurrencyList.Returns(new Dictionary<int, string>());
//            _currencyValidation.IsValidBaseCurrency(metaModel.BaseCurrency).Returns(true);
//            _currencyValidation.IsValidCurrency(metaModel.CurrencyId, Arg.Any<Dictionary<int, string>>()).Returns(true);


//            // Action
//            var result = await controller.MixParlay(metaModel);
//            var page = result.GetValue("page");
//            var total = result.GetValue("total");
//            var records = result.GetValue("records");
//            var rows = result.GetValue("rows");

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(page, Is.EqualTo(pageIndex));
//                Assert.That(total, Is.EqualTo(totalPage));
//                Assert.That(records, Is.EqualTo(totalRows));
//                Assert.That(rows, Is.EqualTo(ticketText));
//            });
//        }

//        [TestCase(1, 20, 50, 3)]
//        public async Task MixParlay_WhenCallWithNotValidCurrency_ReturnNull(int pageIndex, int pageSize, int totalRows, int totalPage)
//        {
//            // Arrange
//            var controller = CreateController();
//            var metaModel = FakeHelper.Create<MixParlayMetaModel>();
//            metaModel.Page = pageIndex;
//            metaModel.Rows = pageSize;
//            var ticketResult = new TicketResult { TotalRows = totalRows };
//            _betListService.GetBetListData((BetListMetaModel)default).ReturnsForAnyArgs(ticketResult);
//            _balanceUpDataAccess.GetBalanceUp().Returns(FakeHelper.Create<BalanceUp>());
//            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(true);

//            var ticketText = FakeHelper.CreateMany<TicketText>(2);
//            _ticketBuilderService.BuildTicketText(ticketResult, Arg.Any<BetListMetaModel>(), Arg.Any<int>(), Arg.Any<BuildTicketOption>()).Returns(ticketText.ToList());

//            _contextService.CurrencyList.Returns(new Dictionary<int, string>());
//            _currencyValidation.IsValidBaseCurrency(metaModel.BaseCurrency).Returns(false);
//            _currencyValidation.IsValidCurrency(metaModel.CurrencyId, Arg.Any<Dictionary<int, string>>()).Returns(true);

//            // Action
//            var result = await controller.MixParlay(metaModel);

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(result, Is.Null);
//            });
//        }

//        private MatchWinLossBetListController CreateController() 
//            => new (_contextService, _authorizationService, _betListService, _ticketBuilderService, _subsidiaryService, _balanceUpDataAccess, _currencyValidation);

//    }
//}
