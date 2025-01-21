using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Services;
using Fanex.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NSubstitute;
using System.Net;
using System.Security.Claims;

namespace Fanex.Athena.Common.Service.UnitTests
{
    public class ContextServiceTests
    {
        private ContextService _contextService;
        private IHttpContextAccessor _httpContextAccessorMock;
        private HttpContext _httpContextMock;
        private ClaimsPrincipal _userMock;
        private static readonly string[] stringArray = new string[] { "Role1", "Role2" };

        [SetUp]
        public void Setup()
        {
            _httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            _httpContextMock = Substitute.For<HttpContext>();
            _userMock = Substitute.ForPartsOf<ClaimsPrincipal>();

            _httpContextAccessorMock.HttpContext.Returns(_httpContextMock);
            _httpContextMock.User.Returns(_userMock);

            _contextService = new ContextService(_httpContextAccessorMock);
        }

        [TearDown]
        public void TearDown()
        {
            _httpContextAccessorMock = null;
            _httpContextMock = null;
            _userMock = null;
            _contextService = null;
        }

        [Test]
        public void IsAllowedCustomerComm_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("IsAllowedCustomerComm", expectedValue.ToString());

            // Act
            var actualValue = _contextService.IsAllowedCustomerComm;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsEnableBA_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("IsEnableBA", expectedValue.ToString());

            // Act
            var actualValue = _contextService.IsEnableBA;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsEnableSingleWallet_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("IsEnableSingleWallet", expectedValue.ToString());

            // Act
            var actualValue = _contextService.IsEnableSingleWallet;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void SiteName_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = "Test Site";
            SetupUserClaim("SiteName", expectedValue);

            // Act
            var actualValue = _contextService.SiteName;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void LicenseeId_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = 123;
            SetupUserClaim("LicenseeId", expectedValue.ToString());

            // Act
            var actualValue = _contextService.LicenseeId;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void SpecialPlatform_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = 456;
            SetupUserClaim("SpecialPlatform", expectedValue.ToString());

            // Act
            var actualValue = _contextService.SpecialPlatform;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void SpecialPlatformType_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = 789;
            SetupUserClaim("SpecialPlatformType", expectedValue.ToString());

            // Act
            var actualValue = _contextService.SpecialPlatformType;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void UserName_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = "testuser";
            SetupUserClaim("UserID", expectedValue);

            // Act
            var actualValue = _contextService.UserName;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void UserID_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = 12345;
            SetupUserClaim("UserSEQ", expectedValue.ToString());

            // Act
            var actualValue = _contextService.UserID;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsFakeLicensee_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("IsFakeLicensee", expectedValue.ToString());

            // Act
            var actualValue = _contextService.IsFakeLicensee;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsAdministrator_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("Administrator", expectedValue.ToString());

            // Act
            var actualValue = _contextService.IsAdministrator;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void SiteDBServer_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = "TestDBServer";
            SetupUserClaim("SiteDBServer", expectedValue);

            // Act
            var actualValue = _contextService.SiteDBServer;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void SiteDBServer_Should_Return_Empty_String_When_GetClaimValue_Returns_Empty_String()
        {
            // Arrange
            SetupUserClaim("SiteDBServer", string.Empty);

            // Act
            var actualValue = _contextService.SiteDBServer;

            // Assert
            Assert.That(actualValue, Is.Empty);
        }

        [Test]
        public void OfferedSubProductList_Should_Return_Correct_Values()
        {
            // Arrange
            var expectedValues = new List<string> { "SubProduct1", "SubProduct2", "SubProduct3" };
            var offeredSubProductList = string.Join(",", expectedValues);
            SetupUserClaim("OfferedSubProducts", offeredSubProductList);

            // Act
            var actualValues = _contextService.OfferedSubProductList;

            // Assert
            Assert.That(actualValues, Is.EquivalentTo(expectedValues));
        }

        [Test]
        public void OfferedSubProductList_ShouldReturnEmptyList_WhenOfferedSubProductsIsNull()
        {
            // Arrange
            var expectedValues = new List<string>();

            // Act
            var actualValues = _contextService.OfferedSubProductList;

            // Assert
            Assert.That(actualValues, Is.EquivalentTo(expectedValues));
        }

        [Test]
        public void IsInRoles_Should_Return_Correct_Value()
        {
            // Arrange
            var roles = new string[] { "Role1", "Role2", "Role3" };
            var expectedValue = true;
            SetUpRoles(roles);
            _userMock.IsInRole("Role1").Returns(expectedValue);

            // Act
            var actualValue = _contextService.IsInRoles("Role1");

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsInRoles_Should_Return_False_When_No_Roles_Provided()
        {
            // Arrange
            var roles = new string[] { };
            var expectedValue = false;
            SetUpRoles(roles);
            // Act
            var actualValue = _contextService.IsInRoles("Role4");

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsInRoles_Should_Return_False_When_No_Matching_Roles()
        {
            // Arrange
            var roles = new string[] { "Role1", "Role2", "Role3" };
            var expectedValue = false;
            SetUpRoles(roles);

            // Act
            var actualValue = _contextService.IsInRoles("Role4");

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ClientIP_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = "127.0.0.1";
            var _mockConnection = Substitute.ForPartsOf<ConnectionInfo>();
            _httpContextMock.Connection.Returns(_mockConnection);
            _httpContextMock.Connection.RemoteIpAddress.Returns(IPAddress.Parse(expectedValue));

            // Act
            var actualValue = _contextService.ClientIP;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void AgentList_Should_Return_Correct_Values()
        {
            // Arrange
            var expectedValue = new CurrencyAgentInfo
            {
                AgentId = [1, 2],
                CurrencyId = 1,
                Currency = "USD"
            };
            SetupUserClaim("CurrencyAgent", JsonConvert.SerializeObject(expectedValue));

            // Act
            var actualValues = _contextService.AgentList;

            // Assert
            Assert.That(actualValues.Keys.First(), Is.EqualTo(expectedValue.CurrencyId));
            Assert.That(actualValues.Values.First(), Is.EquivalentTo(expectedValue.AgentId));
        }

        [Test]
        public void CurrencyList_Should_Return_Correct_Values()
        {
            // Arrange
            var expectedValue = new CurrencyAgentInfo
            {
                AgentId = [1, 2],
                CurrencyId = 1,
                Currency = "USD"
            };
            SetupUserClaim("CurrencyAgent", JsonConvert.SerializeObject(expectedValue));

            // Act
            var actualValues = _contextService.CurrencyList;

            // Assert
            Assert.That(actualValues.Keys.First(), Is.EqualTo(expectedValue.CurrencyId));
            Assert.That(actualValues.Values.First(), Is.EquivalentTo(expectedValue.Currency));
        }

        [Test]
        public void ForceIgnoreSharding_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = true;
            var _mockRequest = Substitute.ForPartsOf<HttpRequest>();
            _mockRequest.Headers.Returns(new HeaderDictionary { { "X-IgnoreSharding", bool.TrueString } });
            _httpContextMock.Request.Returns(_mockRequest);

            // Act
            var actualValue = _contextService.ForceIgnoreSharding;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void SubsidiaryViewType_Should_Return_NotManagement_When_VendorID_Is_Null_Or_Empty()
        {
            // Arrange
            SetupUserClaim("VendorID", string.Empty);

            // Act
            var actualValue = _contextService.SubsidiaryViewType;

            // Assert
            Assert.That(actualValue, Is.EqualTo(Constant.SubsidiaryViewType.NotManagement));
        }

        [Test]
        public void SubsidiaryViewType_Should_Return_MasterManagement_When_IsEnabledSubsidiary_Is_True_And_UserLevel_Is_Less_Than_SubsidaryLevel_And_IsMasterLicensee_Is_True_And_WhiteLabelID_Is_Null()
        {
            // Arrange
            const int SubsidiaryLevel = 2;
            SetupUserClaim("VendorID", "TestVendorID");
            SetupUserClaim("WhiteLabelID", (-1).ToString());
            SetupUserClaim("IsEnabledSubsidiary", true.ToString());
            SetupUserClaim("UserLevel", (SubsidiaryLevel - 1).ToString());
            SetupUserClaim("IsMasterLicensee", true.ToString());

            // Act
            var actualValue = _contextService.SubsidiaryViewType;

            // Assert
            Assert.That(actualValue, Is.EqualTo(Constant.SubsidiaryViewType.MasterManagement));
        }

        [Test]
        public void SubsidiaryViewType_Should_Return_NormalManagement_When_IsEnabledSubsidiary_Is_True_And_UserLevel_Is_Less_Than_SubsidaryLevel_And_IsMasterLicensee_Is_False_And_WhiteLabelID_Is_Null()
        {
            // Arrange
            const int SubsidiaryLevel = 2;
            SetupUserClaim("VendorID", "TestVendorID");
            SetupUserClaim("WhiteLabelID", 0.ToString());
            SetupUserClaim("IsEnabledSubsidiary", true.ToString());
            SetupUserClaim("UserLevel", (SubsidiaryLevel - 1).ToString());
            SetupUserClaim("IsMasterLicensee", false.ToString());

            // Act
            var actualValue = _contextService.SubsidiaryViewType;

            // Assert
            Assert.That(actualValue, Is.EqualTo(Constant.SubsidiaryViewType.NormalManagement));
        }

        [Test]
        public void SubsidiaryViewType_Should_Return_NotManagement_When_IsEnabledSubsidiary_Is_False()
        {
            // Arrange
            SetupUserClaim("VendorID", "TestVendorID");
            SetupUserClaim("IsEnabledSubsidiary", false.ToString());

            // Act
            var actualValue = _contextService.SubsidiaryViewType;

            // Assert
            Assert.That(actualValue, Is.EqualTo(Constant.SubsidiaryViewType.NotManagement));
        }

        [Test]
        public void SubsidiaryViewType_Should_Return_NotManagement_When_UserLevel_Is_Greater_Than_SubsidaryLevel()
        {
            // Arrange
            const int SubsidiaryLevel = 2;
            SetupUserClaim("VendorID", "TestVendorID");
            SetupUserClaim("IsEnabledSubsidiary", true.ToString());
            SetupUserClaim("UserLevel", (SubsidiaryLevel + 1).ToString());

            // Act
            var actualValue = _contextService.SubsidiaryViewType;

            // Assert
            Assert.That(actualValue, Is.EqualTo(Constant.SubsidiaryViewType.NotManagement));
        }

        [Test]
        public void IsShowIP_Should_Return_Correct_Value()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("IsShowIP", expectedValue.ToString());

            // Act
            var actualValue = _contextService.IsShowIP;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsShowIpvn_Should_Return_Correct_Value_When_IsShowIP_Is_True_And_IsAdministrator_Is_True()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("IsShowIP", expectedValue.ToString());
            SetupUserClaim("Administrator", true.ToString());
            SetupUserClaim("name", "testuser");

            // Act
            var actualValue = _contextService.IsShowIpvn;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsShowIpvn_Should_Return_Correct_Value_When_IsShowIP_Is_True_And_IsInRoles_Contains_BetlistViewIpvn()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("IsShowIP", expectedValue.ToString());
            SetupUserClaim("name", "testuser");
            SetUpRoles([Roles.BetlistViewIpvn]);

            // Act
            var actualValue = _contextService.IsShowIpvn;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsShowIpvn_Should_Return_Correct_Value_When_IsShowIP_Is_True_And_IsInRoles_Contains_PortalSuperAdmin()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("IsShowIP", expectedValue.ToString());
            SetupUserClaim("name", "testuser");
            SetUpRoles([Roles.PortalSuperAdmin]);

            // Act
            var actualValue = _contextService.IsShowIpvn;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsShowIpvn_Should_Return_Correct_Value_When_IsShowIP_Is_True_And_IsInRoles_Does_Not_Contain_BetlistViewIpvn_Or_PortalSuperAdmin_And_IsHaveConfigShowIpVnInStatementService_Is_True()
        {
            // Arrange
            var expectedValue = true;
            SetupUserClaim("IsShowIP", expectedValue.ToString());
            SetupUserClaim("IShowIPVN", true.ToString());
            SetupUserClaim("name", "testuser");
            SetUpRoles([Roles.StatementServiceRoleUser]);

            // Act
            var actualValue = _contextService.IsShowIpvn;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsShowIpvn_Should_Return_Correct_Value_When_IsShowIP_Is_True_And_IsInRoles_Does_Not_Contain_BetlistViewIpvn_Or_PortalSuperAdmin_And_IsHaveConfigShowIpVnInStatementService_Is_False()
        {
            // Arrange
            var expectedValue = false;
            SetupUserClaim("IsShowIP", expectedValue.ToString());
            SetupUserClaim("IShowIPVN", false.ToString());
            SetUpRoles([Roles.StatementServiceRoleUser]);

            // Act
            var actualValue = _contextService.IsShowIpvn;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void IsShowIpvn_Should_Return_Correct_Value_When_IsShowIP_Is_False()
        {
            // Arrange
            var expectedValue = false;
            SetupUserClaim("IsShowIP", expectedValue.ToString());

            // Act
            var actualValue = _contextService.IsShowIpvn;

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        private void SetupUserClaim(string claimType, string claimValue)
        {
            var claims = new List<Claim>();
            if (_userMock.Claims == null || _userMock.Claims.Count() == 0)
            {
                _userMock.Claims.Returns(new List<Claim>());
            }
            ((List<Claim>)_userMock.Claims).Add(new Claim(claimType, claimValue));
            _userMock.FindFirst(claimType).Returns(new Claim(claimType, claimValue));
        }

        private void SetUpRoles(IEnumerable<string> roles)
        {
            roles.ForEach(_ =>
            {
                _userMock.IsInRole(_).Returns(true);
                SetupUserClaim(ClaimTypes.Role, _);
            });
        }


        protected internal class CurrencyAgentInfo
        {
            public int[] AgentId { get; set; }

            public int CurrencyId { get; set; }

            public string Currency { get; set; }
        }
    }
}
