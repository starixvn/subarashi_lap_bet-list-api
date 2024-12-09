using AutoFixture;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Resources;
using Fanex.Athena.TestHelper;
using NSubstitute;

namespace Fanex.Athena.Common.DataAccess.UnitTests
{
    public class ProductAccessTests : BaseDataAccessHelperTest<ProductAccess>
    {
        private const string Usp = "usp_Product_Get";

        private Fixture _fixture => new Fixture();

        [Test]
        public void GetProduct_WhenCall_ReturnResultCorrectly()
        {
            // Arrange
            var model = _fixture.Create<Product>();
            model.ProductCode = "NG";
            var objectMapper = new Dictionary<string, object>
            {
                { "ProductID", model.ProductID },
                { "productprefix", model.ProductCode },
                { "comm", model.AllowCommission },
                { "winloss", model.AllowWinloss },
                { "IsOfferedSingleWallet", model.IsOfferedSingleWallet }
            };

            MapReaderData(objectMapper);
            Reader.Read().Returns(true, false);

            // Action
            var result = DataAccess.GetProductAccessList(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result[0].ProductCode, Is.EqualTo("SB"));
                Assert.That(result[0].IsOfferedSingleWallet, Is.True);
                Assert.That(result[0].ProductName, Is.EqualTo(ViewResources.Sportsbook));
                Assert.That(result[1].ProductID, Is.EqualTo(model.ProductID));
                Assert.That(result[1].ProductCode, Is.EqualTo(model.ProductCode));
                Assert.That(result[1].AllowCommission, Is.EqualTo(model.AllowCommission));
                Assert.That(result[1].AllowWinloss, Is.EqualTo(model.AllowWinloss));
                Assert.That(result[1].IsOfferedSingleWallet, Is.EqualTo(model.IsOfferedSingleWallet));
            });
        }
    }
}
