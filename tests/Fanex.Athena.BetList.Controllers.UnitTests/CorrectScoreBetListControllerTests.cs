using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.BetList.Service.Optiions;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Collections;
using Fanex.Athena.TestHelper;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    public class CorrectScoreBetListControllerTests
    {
        private IContextService _contextService;
        private IAuthorizationService _authorizationService;
        private IBetListService _betListService;
        private ITicketBuilderService _ticketBuilderService;
        private ISubsidiaryService _subsidiaryService;
        private IBalanceUpDataAccess _balanceUpDataAccess;

        [SetUp]
        public void SetUp()
        {
            _contextService = Substitute.For<IContextService>();
            _authorizationService = Substitute.For<IAuthorizationService>();
            _betListService = Substitute.For<IBetListService>();
            _ticketBuilderService = Substitute.For<ITicketBuilderService>();
            _subsidiaryService = Substitute.For<ISubsidiaryService>();
            _balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
        }

        [TearDown]
        public void TearDown()
        {
            _contextService = null;
            _authorizationService = null;
            _betListService = null;
            _ticketBuilderService = null;
            _subsidiaryService = null;
            _balanceUpDataAccess = null;
        }

        [Test]
        public async Task GetBetList_HasMainTicket_ReturnCorrectLeagueName()
        {
            var controller = CreateController();

            // Arrange
            var metaModel = FakeHelper.Create<BetListMetaModel>();

            var ticket = FakeHelper.Create<LrfTicket>();
            var ticketResult = new TicketResult
            {
                SumWinLoss = FakeHelper.Create<decimal>(),
                SumPlayerComm = FakeHelper.Create<decimal>(),
                SumLicenseeWL = FakeHelper.Create<decimal>(),
                SumComm = FakeHelper.Create<decimal>(),
                SumBetEffective = FakeHelper.Create<decimal>(),

                MainTicketList = new SortableList<LrfTicket> { ticket }
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2).ToList();
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, Arg.Any<int>(), Arg.Any<BuildTicketOption>()).Returns(ticketText);

            var leagueName = FakeHelper.Create<string>();
            _betListService.GetLeagueName(ticket.LeagueId).Returns(leagueName);

            _authorizationService.HasPermissionOnData(default, default ,default).ReturnsForAnyArgs(true);
            _balanceUpDataAccess.GetBalanceUp().Returns(FakeHelper.Create<BalanceUp>());

            // Action
            var result = await controller.GetData(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Main, Is.EqualTo(ticketText));
                Assert.That(result.Total.SumWinLoss, Is.EqualTo(ticketResult.SumWinLoss + ticketResult.SumPlayerComm));
                Assert.That(result.Total.SumLicenseeWl, Is.EqualTo(ticketResult.SumLicenseeWL + ticketResult.SumComm));
                Assert.That(result.Total.SumBetEffective, Is.EqualTo(ticketResult.SumBetEffective));
                Assert.That(result.Total.SumStake, Is.EqualTo(Math.Round(ticket.Stake, 2)));
                Assert.That(result.LeagueName, Is.EqualTo(leagueName));
                Assert.That(metaModel.UspKey, Is.EqualTo("Lrf_TotalBets_Sel430"));
            });

        }

        private CorrectScoreBetListController CreateController()
            => new(_contextService, _betListService, _ticketBuilderService, _balanceUpDataAccess, _authorizationService, _subsidiaryService);
    }
}
