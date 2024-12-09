using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.TestHelper;
using NUnit.Framework;
using NSubstitute;


namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    public class BetListByProductControllerTests
    {
        private IBetListByProductService _service;

        private ITicketBuilderService _ticketBuilderService;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            _service = Substitute.For<IBetListByProductService>();
            _ticketBuilderService = Substitute.For<ITicketBuilderService>();
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            _ticketBuilderService = null;
            _service = null;
        }

        [Test]
        public async Task Index_WhenCall_QueryAndReturnCorrectViewModel()
        {


            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BetListByProductMetaModel>();

            var ticketResult = new TicketResult();
            _service.QueryAsync(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2).ToList();
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, metaModel.HasPlayerCom).Returns(ticketText);

            // Action
            var result = await controller.Index(metaModel);
            var data = result.GetValue("Data");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(data, Is.EqualTo(ticketText));
            });

        }

        private BetListByProductController CreateController()
        {
            return new BetListByProductController(
                _service,
                _ticketBuilderService);
        }
    }
}
