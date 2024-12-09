using Fanex.Athena.Result.Model.MetaModel.Keno;
using Fanex.Athena.Result.Service;
using System.Net;
using Fanex.Athena.Result.Model.Const;
using Fanex.Athena.Result.Model.DTO.Keno;
using Fanex.Athena.TestHelper.Extensions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Fanexe.Athena.Result.Service.Tests
{
    public class KenoApiTests
    {
        private MockHttpMessageHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _handler = new MockHttpMessageHandler();
        }

        [TearDown]
        public void TearDown()
        {
            _handler.Dispose();
            _handler = null;
        }

        [Test]
        public async Task ExtendToken_ExtendTokenUrlNull_ThrowException()
        {
            // Arrange
            var setting = new KenoResultMetaModel();
            var service = CreateService(_handler.ToHttpClient());

            // Assert
            var exception =
                Assert.ThrowsAsync<WebException>(async () => await service.ExtendToken(setting, string.Empty));
            Assert.That(exception.Message.Contains("Value cannot be null"), Is.True);
        }

        [Test]
        public void ExtendToken_ApiReturnEmpty_ThrowException()
        {
            // Arrange
            const string ExtendTokenUrl = "http://localhost";
            var setting = new KenoResultMetaModel
            {
                ExtendTokenUrl = ExtendTokenUrl
            };
            _handler.When(ExtendTokenUrl).Respond("application/json", string.Empty);

            var service = CreateService(_handler.ToHttpClient());
            // Action
            AsyncTestDelegate testDelegate = async () => await service.ExtendToken(setting, string.Empty);

            // Assert
            var exception = Assert.ThrowsAsync<WebException>(testDelegate);
            Assert.That(exception.Message.Contains("Couldn't get authorized data from: " + ExtendTokenUrl), Is.True);
        }

        [Test]
        public async Task ExtendToken_WhenCall_ReturnDataFromApi()
        {
            // Arrange
            const string UserId = "1234";
            const int ExpectedErrorCode = 888;
            const string ExpectedErrorMsg = "Noway";
            var setting = new KenoResultMetaModel
            {
                Vendor = "abc",
                VenderKey = "def",
                VendorProduct = ResultVendorProductTypes.Keno,
                ExtendTokenUrl = "http://locahhost/{0}{1}{2}{3}"
            };
            var extendedUrl = string.Format(setting.ExtendTokenUrl, setting.Vendor, setting.VenderKey, UserId,
                setting.VendorProduct);
            _handler.When(extendedUrl).Respond("application/json",
                $"{{ErrorCode: {ExpectedErrorCode}, ErrorMsg: \"{ExpectedErrorMsg}\"}}");

            var service = CreateService(_handler.ToHttpClient());

            // Action
            var result = await service.ExtendToken(setting, UserId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.ErrorCode, Is.EqualTo(ExpectedErrorCode));
                Assert.That(result.ErrorMsg, Is.EqualTo(ExpectedErrorMsg));
            });
        }

        [Test]
        public void CreateKenoResultUrl_AuthorizeFail_ThrowException()
        {
            // Arrange
            const string UserId = "heneken";
            const string ExpectedErrorMsg = "Whatthehell";
            var setting = new KenoResultMetaModel();

            var service = Substitute.ForPartsOf<KenoApi>(_handler.ToHttpClient());

            service.WhenForAnyArgs(x => x.InvokeProtectedMethod("Authorize", null, null)).DoNotCallBase();
            service.InvokeProtectedMethod("Authorize", setting, UserId).Returns(Task.FromResult(new KenoAuthorization
            {
                ErrorCode = 11,
                ErrorMsg = ExpectedErrorMsg
            }));

            // Action
            AsyncTestDelegate testDelegate = async () =>
                await service.CreateKenoResultAddress(setting, UserId, DateTime.MinValue);

            // Assert
            var exception = Assert.ThrowsAsync<WebException>(testDelegate);
            Assert.That(exception.Message.Contains(ExpectedErrorMsg), Is.True);
        }

        [Test]
        public async Task CreateKenoResultUrl_WhenCall_ReturnResultUrl()
        {
            // Arrange
            const string UserId = "heneken";
            var setting = new KenoResultMetaModel
            {
                SiteID = "grabbike",
                VendorProduct = ResultVendorProductTypes.Keno,
                KickOutUrl = "xxx.com",
                ResultUrl = "TestUrl?data=1"
            };

            var service = Substitute.ForPartsOf<KenoApi>(_handler.ToHttpClient());

            service.WhenForAnyArgs(x => x.InvokeProtectedMethod("Authorize", null, null)).DoNotCallBase();
            service.InvokeProtectedMethod("Authorize", setting, UserId).Returns(Task.FromResult(new KenoAuthorization
            {
                Data = new KenoAuthorizedData { Authorization = "uber" }
            }));

            // Action
            var resultUrl = await service.CreateKenoResultAddress(setting, UserId, DateTime.MinValue);

            // Assert
            Assert.That(resultUrl,
                Is.EqualTo(
                    "TestUrl?data=1&Authorization=uber&cust=Keno&siteID=grabbike&action=R&lang=&date=00010101&redirectURL=xxx.com"));
        }

        [Test]
        public async Task Authorize_WhenCall_ReturnResultUrl()
        {
            // Arrange
            const string UserId = "heneken";
            var setting = new KenoResultMetaModel
            {
                SiteID = "grabbike",
                VendorProduct = ResultVendorProductTypes.Keno,
                KickOutUrl = "xxx.com",
                ResultUrl = "TestUrl"
            };
            var service = Substitute.ForPartsOf<KenoApi>(_handler.ToHttpClient());

            service.WhenForAnyArgs(x => x.InvokeProtectedMethod("Authorize", null, null)).DoNotCallBase();
            service.InvokeProtectedMethod("Authorize", setting, UserId).Returns(Task.FromResult(new KenoAuthorization
            {
                Data = new KenoAuthorizedData { Authorization = "uber" }
            }));

            // Action
            var resultUrl = await service.CreateKenoResultAddress(setting, UserId, DateTime.MinValue);

            // Assert
            Assert.That(resultUrl,
                Is.EqualTo(
                    "TestUrl?Authorization=uber&cust=Keno&siteID=grabbike&action=R&lang=&date=00010101&redirectURL=xxx.com"));
        }

        [Test]
        public async Task Authorize_ExtendTokenUrlNull_ThrowException()
        {
            // Arrange
            var setting = new KenoResultMetaModel();
            var service = CreateService(_handler.ToHttpClient());

            // Action
            AsyncTestDelegate testDelegate = async () =>
                await service.InvokeAsyncProtectedMethod<KenoApi, KenoAuthorization>("Authorize", setting,
                    string.Empty);

            // Assert
            var exception = Assert.ThrowsAsync<WebException>(testDelegate);
            Assert.That(exception.Message.Contains("Value cannot be null"), Is.True);
        }

        [Test]
        public void Authorize_ApiReturnEmpty_ThrowException()
        {
            // Arrange
            var setting = new KenoResultMetaModel
            {
                AuthorizedUrl = "http://localhost"
            };
            _handler.When(setting.AuthorizedUrl).Respond("application/json", string.Empty);
            var service = CreateService(_handler.ToHttpClient());
            // Action
            AsyncTestDelegate testDelegate = async () =>
                await service.InvokeAsyncProtectedMethod<KenoApi, KenoAuthorization>("Authorize", setting,
                    string.Empty);

            // Assert
            var exception = Assert.ThrowsAsync<WebException>(testDelegate);
            Assert.That(exception.Message.Contains("Couldn't get authorized data from: " + setting.AuthorizedUrl),
                Is.True);
        }

        [Test]
        public async Task Authorize_WhenCall_ReturnDataFromApi()
        {
            // Arrange
            const string UserId = "1234";
            const int ExpectedErrorCode = 888;
            const string ExpectedErrorMsg = "Noway";
            var setting = new KenoResultMetaModel
            {
                Vendor = "abc",
                VenderKey = "def",
                VendorProduct = ResultVendorProductTypes.Keno,
                AuthorizedUrl = "http://localhost/{0}{1}{2}{3}"
            };
            var extendedUrl = string.Format(setting.AuthorizedUrl, setting.Vendor, setting.VenderKey, UserId,
                setting.VendorProduct);
            _handler.When(extendedUrl).Respond("application/json",
                $"{{ErrorCode: {ExpectedErrorCode}, ErrorMsg: \"{ExpectedErrorMsg}\"}}");

            var service = CreateService(_handler.ToHttpClient());
            // Action
            var result =
                await service.InvokeAsyncProtectedMethod<KenoApi, KenoAuthorization>("Authorize", setting, UserId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.ErrorCode, Is.EqualTo(ExpectedErrorCode));
                Assert.That(result.ErrorMsg, Is.EqualTo(ExpectedErrorMsg));
            });
        }

        private KenoApi CreateService(HttpClient client)
            => Substitute.ForPartsOf<KenoApi>(client);
    }
}