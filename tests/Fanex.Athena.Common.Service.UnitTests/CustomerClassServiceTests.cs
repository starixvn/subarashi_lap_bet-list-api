using AutoFixture;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;

namespace Fanex.Athena.Common.Service.UnitTests
{
    public class CustomerClassServiceTests
    {
        private readonly Fixture _fixture = new();

        private IMemoryCacheService _cacheServiceMock;
        private IHttpContextAccessor _httpContextAccessorMock;
        private IAppSettings _appSettingsMock;
        private ILogger<CustomerClassService> _loggerMock;
        private ICustomerClassService _customerClassService;
        private MockHttpMessageHandler _handler;
        private HttpClient _httpClient;


        [SetUp]
        public void Setup()
        {
            _cacheServiceMock = Substitute.For<IMemoryCacheService>();
            _httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            _appSettingsMock = Substitute.For<IAppSettings>();
            _loggerMock = Substitute.For<ILogger<CustomerClassService>>();
            _handler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_handler);

            SetUpSetting();

            _customerClassService = new CustomerClassService(
                _cacheServiceMock,
                _httpContextAccessorMock,
                _appSettingsMock,
                _httpClient,
                _loggerMock);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
            _handler.Dispose();
            _cacheServiceMock = null;
            _httpContextAccessorMock = null;
            _appSettingsMock = null;
            _loggerMock = null;
            _httpClient = null;
            _customerClassService = null;
            _handler = null;
        }


        [Test]
        public async Task GetAllCustomerClassId_WhenCached_ShouldReturnAllCustomerClassIds()
        {
            // Arrange
            var customerClassCategories = SourceTest.OrderBy(_ => _.CustomerClass).ToList();
            _cacheServiceMock.Get<List<CustomerClassCategoryModel>>(Arg.Any<string>()).Returns(customerClassCategories);

            // Act
            var result = await _customerClassService.GetAllCustomerClassId();

            // Assert
            Assert.That(result.Count, Is.EqualTo(customerClassCategories.Count() + 1));
            Assert.That(result.First(), Is.EqualTo("0"));
            Assert.That(result.ElementAt(1), Is.EqualTo(customerClassCategories.ElementAt(0).CustomerClass.ToString()));
            Assert.That(result.ElementAt(2), Is.EqualTo(customerClassCategories.ElementAt(1).CustomerClass.ToString()));
            Assert.That(result.ElementAt(3), Is.EqualTo(customerClassCategories.ElementAt(2).CustomerClass.ToString()));
        }

        [Test]
        public async Task GetAllCustomerClassId_WhenNotCachedAndAPIReturnNotEmptySource_CachedSourceAndReturnAllCustomerClassIds()
        {
            // Arrange
            var customerClassCategories = SourceTest.OrderBy(_ => _.CustomerClass).ToList();

            _cacheServiceMock.Get<List<CustomerClassCategoryModel>>(Arg.Any<string>()).Returns((List<CustomerClassCategoryModel>)null);
            _handler.When(_appSettingsMock.CustomerClassCategoryApiUrl)
                .Respond(HttpStatusCode.OK, JsonContent.Create(customerClassCategories));
            // Act
            var result = await _customerClassService.GetAllCustomerClassId();

            // Assert
            Assert.That(result.Count, Is.EqualTo(customerClassCategories.Count() + 1));
            Assert.That(result.First(), Is.EqualTo("0"));
            Assert.That(result.ElementAt(1), Is.EqualTo(customerClassCategories.ElementAt(0).CustomerClass.ToString()));
            Assert.That(result.ElementAt(2), Is.EqualTo(customerClassCategories.ElementAt(1).CustomerClass.ToString()));
            Assert.That(result.ElementAt(3), Is.EqualTo(customerClassCategories.ElementAt(2).CustomerClass.ToString()));
            _cacheServiceMock.Received(1).Set(CacheKey.MemorycacheCustomerClassCategories, Arg.Any<IEnumerable<CustomerClassCategoryModel>>(), Arg.Any<int>());
            _cacheServiceMock.DidNotReceive().Set($"{CacheKey.MemorycacheCustomerClassCategories}_backup", Arg.Any<IEnumerable<CustomerClassCategoryModel>>(), Arg.Any<int>());
        }

        [Test]
        public async Task GetAllCustomerClassId_WhenNotCachedAndAPIReturnEmptySource_ReceiveCacheBackUpCall()
        {
            // Arrange
            var customerClassCategories = SourceTest.OrderBy(_ => _.CustomerClass).ToList();

            _cacheServiceMock.Get<List<CustomerClassCategoryModel>>(Arg.Any<string>()).Returns((List<CustomerClassCategoryModel>)null);
            _handler.When(_appSettingsMock.CustomerClassCategoryApiUrl)
                .Respond(HttpStatusCode.OK, JsonContent.Create(Enumerable.Empty<CustomerClassCategoryModel>()));
            // Act
            var result = await _customerClassService.GetAllCustomerClassId();

            // Assert
            _cacheServiceMock.DidNotReceive().Set(CacheKey.MemorycacheCustomerClassCategories, Arg.Any<IEnumerable<CustomerClassCategoryModel>>(), Arg.Any<int>());
            _cacheServiceMock.Received().Get<List<CustomerClassCategoryModel>>($"{CacheKey.MemorycacheCustomerClassCategories}_backup");
        }

        [Test]
        public async Task GetAllCustomerClassId_ThrowExceptionWhenGetCustomerClass_ReturnOnlyOneCustomerClassId()
        {
            // Arrange
            var customerClassCategories = SourceTest.OrderBy(_ => _.CustomerClass).ToList();

            _cacheServiceMock.Get<List<CustomerClassCategoryModel>>(Arg.Any<string>()).Returns((List<CustomerClassCategoryModel>)null);
            _handler.When(_appSettingsMock.CustomerClassCategoryApiUrl)
                .Throw(new Exception());

            // Act
            var result = await _customerClassService.GetAllCustomerClassId();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First(), Is.EqualTo("0"));
        }

        [Test]
        public async Task GetAllRiskLevel_ShouldReturnAllRiskLevels()
        {
            // Arrange
            var customerClassCategories = SourceTest.OrderBy(_ => _.CustomerClass).ToList();
            _cacheServiceMock.Get<List<CustomerClassCategoryModel>>(Arg.Any<string>()).Returns(customerClassCategories.ToList());

            // Act
            var result = await _customerClassService.GetAllRiskLevel();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(customerClassCategories.GroupBy(_ => _.SabaRiskLevel).Distinct().Count() + 1));
            Assert.That(result.Last().Id, Is.EqualTo((result.Count() - 1).ToString()));
            Assert.That(result.Last().Key, Is.EqualTo(RiskLevel.Uncategorized));
        }

        private List<CustomerClassCategoryModel> SourceTest
            => new List<CustomerClassCategoryModel>
            {
                new CustomerClassCategoryModel { CustomerClass = 1, SabaRiskLevel = "Low" },
                new CustomerClassCategoryModel { CustomerClass = 2, SabaRiskLevel = "Medium" },
                new CustomerClassCategoryModel { CustomerClass = 3, SabaRiskLevel = "High" }
            };

        private void SetUpSetting()
        {
            _appSettingsMock.CustomerClassCategoryApiUrl.Returns("http://abc.com/api/customer-class-category");
        }

    }
}
