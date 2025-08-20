namespace Fanex.Athena.Common.Site.Tests.Attributes
{
    using Constant;
    using Core.Utils;
    using DataAccess.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using Services.Interfaces;
    using Site.Attributes;

    public class AutoBindMetaModelAttributeTests
    {
        private ActionExecutingContext _context;
        private IMetaModelBindingService _bindingServiceMock;
        private IContextService _contextServiceMock;
        private ISubsidiaryService _subsidiaryServiceMock;
        private IBalanceUpDataAccess _balanceUpDataAccessMock;

        [SetUp]
        public void SetUp()
        {
            _context = CreateExecutingContext();

            _bindingServiceMock = Substitute.For<IMetaModelBindingService>();
            _contextServiceMock = Substitute.For<IContextService>();
            _subsidiaryServiceMock = Substitute.For<ISubsidiaryService>();
            _balanceUpDataAccessMock = Substitute.For<IBalanceUpDataAccess>();
        }

        [TearDown]
        public void TearDown()
        {
            _context = null;
            _bindingServiceMock = null;
            _contextServiceMock = null;
            _subsidiaryServiceMock = null;
            _balanceUpDataAccessMock = null;

        }

        [Test]
        public async Task OnActionExecutionAsync_BindAgentWithoutApplySubsidiaryBusiness_CallsBindAgent()
        {
            // Arrange
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: true,
                bindPlayerCom: false,
                bindProductCodeList: false,
                applySubsidiaryBusiness: false,
                bindShowingBetFromIP: false,
                isBindSpecialPlatform: false,
                isBindBalanceUpAndMoveBet: false
            );

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            await _subsidiaryServiceMock.DidNotReceiveWithAnyArgs().GetAgentList(default);
            _bindingServiceMock.Received(1).BindAgentXml(Arg.Any<Dictionary<string, object>>(), Arg.Any<IDictionary<int, int[]>>());
        }

        [Test]
        public async Task OnActionExecutionAsync_BindAgent_CallsBindAgent_WithSubsidiaryBusiness()
        {
            // Arrange
            var listSubsidiaries = "subsidiary1,subsidiary2";
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: true,
                bindPlayerCom: false,
                bindProductCodeList: false,
                applySubsidiaryBusiness: true,
                bindShowingBetFromIP: false,
                isBindSpecialPlatform: false,
                isBindBalanceUpAndMoveBet: false
            );

            var subsidiarySearchMetaModel = Substitute.For<ISubsidiarySearchMetaModel>();
            subsidiarySearchMetaModel.SubsidiarySearch.Returns(true);
            subsidiarySearchMetaModel.SubsidiaryList.Returns(listSubsidiaries);
            _subsidiaryServiceMock.IsIgnoredSubsidiarySearch(true, listSubsidiaries).Returns(false);

            _context.ActionArguments.Add("subsidiaryMetaModel", subsidiarySearchMetaModel);

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            await _subsidiaryServiceMock.Received(1).GetAgentList(Arg.Is(listSubsidiaries));
            _bindingServiceMock.Received(1).BindAgentXml(Arg.Any<Dictionary<string, object>>(), Arg.Any<IDictionary<int, int[]>>());
        }

        [TestCase(false, false, false)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, true)]
        public async Task OnActionExecutionAsync_BindPlayerCom_SetsHasPlayerCom(bool isBindPlayerCom, bool isBindPlayerCommFromContextService, bool expectedResult)
        {
            // Arrange
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: false,
                bindPlayerCom: isBindPlayerCom,
                bindProductCodeList: false,
                applySubsidiaryBusiness: false,
                bindShowingBetFromIP: false,
                isBindSpecialPlatform: false,
                isBindBalanceUpAndMoveBet: false
            );

            var playerComMetaModel = Substitute.For<IPlayerComMetaModel>();
            _context.ActionArguments.Add("playerComMetaModel", playerComMetaModel);
            _contextServiceMock.IsAllowedCustomerComm.Returns(isBindPlayerCommFromContextService);

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            Assert.That(expectedResult, Is.EqualTo(playerComMetaModel.HasPlayerCom));
        }

        [Test]
        public async Task OnActionExecutionAsync_WhenModelIsNotIMetaModelShowBetFromIP_NotBindShowIP()
        {
            // Arrange
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: false,
                bindPlayerCom: false,
                bindProductCodeList: false,
                applySubsidiaryBusiness: false,
                bindShowingBetFromIP: true,
                isBindSpecialPlatform: false,
                isBindBalanceUpAndMoveBet: false
            );
            _contextServiceMock.IsShowIP.Returns(true);
            _contextServiceMock.IsShowIpvn.Returns(true);
            dynamic model = new { IsShowIpvn = false, IsShowIp = false };
            _context.ActionArguments.Add("model", model);

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            Assert.That(model.IsShowIp, Is.False);
            Assert.That(model.IsShowIpvn, Is.False);
        }

        [Test]
        public async Task OnActionExecutionAsync_WhenModelIsIMetaModelShowBetFromIP_BindShowIP()
        {
            // Arrange
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: false,
                bindPlayerCom: false,
                bindProductCodeList: false,
                applySubsidiaryBusiness: false,
                bindShowingBetFromIP: true,
                isBindSpecialPlatform: false,
                isBindBalanceUpAndMoveBet: false
            );
            _contextServiceMock.IsShowIP.Returns(true);
            _contextServiceMock.IsShowIpvn.Returns(true);
            var model = Substitute.For<IMetaModelShowBetFromIP>();
            _context.ActionArguments.Add("model", model);

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            Assert.That(model.IsShowIp, Is.True);
            Assert.That(model.IsShowIpvn, Is.True);
        }

        [Test]
        public async Task OnActionExecutionAsync_BindProducListWhenSubProductIsEmpty_ShouldBindProductFromModel()
        {
            // Arrange
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: false,
                bindPlayerCom: false,
                bindProductCodeList: true,
                applySubsidiaryBusiness: false,
                bindShowingBetFromIP: false,
                isBindSpecialPlatform: false,
                isBindBalanceUpAndMoveBet: false
            );
            var offeredSubProductList = Enumerable.Empty<string>();
            _contextServiceMock.OfferedSubProductList.Returns(offeredSubProductList);

            var metaModelProductCodeList = Substitute.For<IMetaModelProductCodeList>();
            metaModelProductCodeList.ProductCodeList = "product1,product2";
            _context.ActionArguments.Add("metaModelProductCodeList", metaModelProductCodeList);

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            Assert.That(metaModelProductCodeList.ProductCodeList, Is.EqualTo(metaModelProductCodeList.OriginalProductCodeList));
            Assert.That(metaModelProductCodeList.ProductCodeList, Is.EqualTo("product1,product2"));
        }

        [Test]
        public async Task OnActionExecutionAsync_BindProducListWhenProductListNotContainSB_ShouldBindProductFromModel()
        {
            // Arrange
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: false,
                bindPlayerCom: false,
                bindProductCodeList: true,
                applySubsidiaryBusiness: false,
                bindShowingBetFromIP: false,
                isBindSpecialPlatform: false,
                isBindBalanceUpAndMoveBet: false
            );
            var offeredSubProductList = new List<string> { "subproduct1", "subproduct2" };
            _contextServiceMock.OfferedSubProductList.Returns(offeredSubProductList);

            var metaModelProductCodeList = Substitute.For<IMetaModelProductCodeList>();
            metaModelProductCodeList.ProductCodeList = "product1,product2";
            _context.ActionArguments.Add("metaModelProductCodeList", metaModelProductCodeList);

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            Assert.That(metaModelProductCodeList.ProductCodeList, Is.EqualTo(metaModelProductCodeList.OriginalProductCodeList));
            Assert.That(metaModelProductCodeList.ProductCodeList, Is.EqualTo("product1,product2"));
        }

        [Test]
        public async Task OnActionExecutionAsync_BindProducListWhenProductListContainSB_ShouldBindProductMergeWithSubProduct()
        {
            // Arrange
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: false,
                bindPlayerCom: false,
                bindProductCodeList: true,
                applySubsidiaryBusiness: false,
                bindShowingBetFromIP: false,
                isBindSpecialPlatform: false,
                isBindBalanceUpAndMoveBet: false
            );
            var offeredSubProductList = new List<string> { "subproduct1", "subproduct2" };
            _contextServiceMock.OfferedSubProductList.Returns(offeredSubProductList);

            var metaModelProductCodeList = Substitute.For<IMetaModelProductCodeList>();
            metaModelProductCodeList.ProductCodeList = "SB,product2";
            _context.ActionArguments.Add("metaModelProductCodeList", metaModelProductCodeList);

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            Assert.That(metaModelProductCodeList.ProductCodeList, Is.EqualTo("SB,product2,subproduct1,subproduct2"));
        }

        [Test]
        public async Task OnActionExecutionAsync_WhenBindSpecialPlatform_BindToModel()
        {
            // Arrange
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: false,
                bindPlayerCom: false,
                bindProductCodeList: false,
                applySubsidiaryBusiness: false,
                bindShowingBetFromIP: false,
                isBindSpecialPlatform: true,
                isBindBalanceUpAndMoveBet: false
            );
            var model = Substitute.For<ISpecialPlatformMetaModel>();
            _context.ActionArguments.Add("model", model);
            _contextServiceMock.SpecialPlatform.Returns(SpecialPlatform.Jollysports);

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            Assert.That(model.SpecialPlatform, Is.EqualTo(_contextServiceMock.SpecialPlatform));
        }

        [Test]
        public async Task OnActionExecutionAsync_WhenBindBalanceUpAndMoveBet_BindToModel()
        {
            // Arrange
            var attribute = new AutoBindMetaModelImplAttribute(
                _bindingServiceMock,
                _contextServiceMock,
                _subsidiaryServiceMock,
                _balanceUpDataAccessMock,
                bindAgent: false,
                bindPlayerCom: false,
                bindProductCodeList: false,
                applySubsidiaryBusiness: false,
                bindShowingBetFromIP: false,
                isBindSpecialPlatform: false,
                isBindBalanceUpAndMoveBet: true
            );
            var model = Substitute.For<IBalanceUpMetaModel>();
            _context.ActionArguments.Add("model", model);

            var balanceUp = new BalanceUp { BalanceUpDay = DateTime.Today, MoveBetsDay = DateTime.Today };
            _balanceUpDataAccessMock.GetBalanceUp().ReturnsForAnyArgs(balanceUp);

            // Act
            await attribute.OnActionExecutionAsync(_context, CreateDelegate());

            // Assert
            Assert.That(model.BalanceUpDay, Is.EqualTo(balanceUp.BalanceUpDay));
            Assert.That(model.MoveBetsDay, Is.EqualTo(balanceUp.MoveBetsDay));
        }

        private ActionExecutionDelegate CreateDelegate() => () => Task.FromResult(new ActionExecutedContext(
           new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
           Array.Empty<IFilterMetadata>(),
           new object())
            );

        private ActionExecutingContext CreateExecutingContext() => new ActionExecutingContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            Array.Empty<IFilterMetadata>(),
            new Dictionary<string, object>(),
            Substitute.For<Controller>()
            );

    }
}
