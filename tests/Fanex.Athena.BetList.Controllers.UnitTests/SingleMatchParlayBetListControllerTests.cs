using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.BetList.Core.App_GlobalResources;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    public class SingleMatchParlayBetListControllerTests : BaseControllerHelperTests
    {
        private const string TextRenderTemplate = "<div class='single-match-parlay-selection'><span class='selection'>{0}</span>{1}</div>";

        private const string StatusTextTemplate = " - <span class='status'>{0}</span>";

        private ICacheRefAdapter _cacheRefAdapter;

        private ISingleMatchParlayDetailDataAccess _dataAccess;

        private IBetListHTMLBuilder _betlistHtmlBuilder;

        [SetUp]
        public void SetUp()
        {
            _cacheRefAdapter = Substitute.For<ICacheRefAdapter>();
            _dataAccess = Substitute.For<ISingleMatchParlayDetailDataAccess>();
            _betlistHtmlBuilder = Substitute.For<IBetListHTMLBuilder>();
        }

        [TearDown]
        public void TearDown()
        {
            _cacheRefAdapter = null;
            _dataAccess = null;
            _betlistHtmlBuilder = null;
        }

        [Test]
        public async Task Detail_NoData_ReturnNull()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<SingleMatchParlayDetailMetaModel>();

            var data = new List<SingleMatchParlayDetail>();
            _dataAccess.GetSingleMatchParlayDetail(metaModel).Returns(Task.FromResult<IList<SingleMatchParlayDetail>>(data));

            // Action
            var result = await controller.Detail(metaModel);

            // Assert
            Assert.That(result, Is.Null);
        }

        [TestCase("running")]
        [TestCase("void")]
        public async Task Detail_TicketIsNotSettled_SelectionStatusIsNotShown(string status)
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<SingleMatchParlayDetailMetaModel>(x => x.Status, status);

            var selection = new SingleMatchParlayDetail { SelectionType = FakeHelper.Create<int>(), Result = BetStatus.Won };
            var data = new List<SingleMatchParlayDetail> { selection };
            _dataAccess.GetSingleMatchParlayDetail(metaModel).Returns(Task.FromResult<IList<SingleMatchParlayDetail>>(data));

            var selectionText = FakeHelper.Create<string>();
            _cacheRefAdapter.GetBetBuilderSelectionNames(Arg.Any<int[]>()).Returns(new[] { selectionText });

            // Action
            var result = await controller.Detail(metaModel);

            // Assert
            Assert.That(result.First(), Is.EqualTo(string.Format(TextRenderTemplate, selectionText, string.Empty)));
        }

        [TestCase("won")]
        [TestCase("lose")]
        [TestCase("refund")]
        public async Task Detail_SelectionIsNotSettled_SelectionStatusIsNotShown(string status)
        {

            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<SingleMatchParlayDetailMetaModel>(x => x.Status, status);

            var selection = new SingleMatchParlayDetail { SelectionType = FakeHelper.Create<int>(), Result = null };
            var data = new List<SingleMatchParlayDetail> { selection };
            _dataAccess.GetSingleMatchParlayDetail(metaModel).Returns(Task.FromResult<IList<SingleMatchParlayDetail>>(data));

            var selectionText = FakeHelper.Create<string>();
            _cacheRefAdapter.GetBetBuilderSelectionNames(Arg.Any<int[]>()).Returns(new[] { selectionText });

            // Action
            var result = await controller.Detail(metaModel);

            // Assert
            Assert.That(result.First(), Is.EqualTo(string.Format(TextRenderTemplate, selectionText, string.Empty)));
        }

        [TestCase("won")]
        [TestCase("lose")]
        [TestCase("refund")]
        public async Task Detail_TicketIsSettled_ShowSelectionStatus(string status)
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<SingleMatchParlayDetailMetaModel>(x => x.Status, status);

            var selection = new SingleMatchParlayDetail { SelectionType = FakeHelper.Create<int>(), Result = BetStatus.Won };
            var data = new List<SingleMatchParlayDetail> { selection };
            _dataAccess.GetSingleMatchParlayDetail(metaModel).Returns(Task.FromResult<IList<SingleMatchParlayDetail>>(data));

            var selectionText = FakeHelper.Create<string>();
            _cacheRefAdapter.GetBetBuilderSelectionNames(Arg.Any<int[]>()).Returns(new[] { selectionText });

            // Action
            var result = await controller.Detail(metaModel);

            // Assert
            Assert.That(result.First(), Is.EqualTo(string.Format(TextRenderTemplate, selectionText, string.Format(StatusTextTemplate, CoreBetList.won))));
        }

        private SingleMatchParlayBetListController CreateController()
            => new SingleMatchParlayBetListController(_cacheRefAdapter, _dataAccess, _betlistHtmlBuilder);

    }
}
