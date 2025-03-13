using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Result.Controller.Constants;
using Fanex.Athena.Result.Controller.ResultPopup;
using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.Result.Core.ThirdParty;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using NSubstitute;

namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    public class PGSoftResultPopupControllerTests
    {
        private IHttpContextAccessor _contextAccessor;
        private IBetlistResultGetterFactory _factory;
        private IIpInfoService _ipInfoService;
        private IThirdPartyResult _thirdPartyResult;

        [SetUp]
        public void SetUp()
        {
            _factory = Substitute.For<IBetlistResultGetterFactory>();
            _thirdPartyResult = Substitute.For<IThirdPartyResult>();
            _ipInfoService = Substitute.For<IIpInfoService>();
            _contextAccessor = Substitute.For<IHttpContextAccessor>();
        }

        [TearDown]
        public void TearDown()
        {
            _factory = null;
            _thirdPartyResult = null;
            _ipInfoService = null;
        }

        [TestCase("")]
        [TestCase("127.0.0.1")]
        public void Index_WhenCall_ReturnCorrectResult(string ipAddress)
        {
            //Arrange
            const string location = "VN";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ThirdPartyMetaModel>(x => x.ClientIp, ipAddress);
            _factory.CreateThirdPartyResultGetter($"{ConfigName.ConfigPrefix}:{ConfigName.PGSoftResult}")
                .Returns(_thirdPartyResult);

            _ipInfoService.GetIpLocation(default).ReturnsForAnyArgs(location);

            //Action
            controller.Index(metaModel);

            //Assert
            _thirdPartyResult.Received().GetResultGET(Arg.Is<ResultRequest>(x => x.Location == location));
        }

        private PGSoftResultPopupController CreateController()
        {
            _contextAccessor.HttpContext.Returns(Substitute.For<HttpContext>());
            _contextAccessor.HttpContext.Connection.Returns(Substitute.For<ConnectionInfo>());
            _contextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(System.Net.IPAddress.Parse("127.0.0.1"));
            return new(_ipInfoService, _factory, _contextAccessor);
        }
    }
}