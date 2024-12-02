using Fanex.Athena.BetList.Service.Extensions;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Resources;
using Fanex.BetList.Core.Entities;
using NSubstitute;

namespace Fanex.Athena.BetList.Service.Tests.Extensions
{
    public class TicketHelperExtensionTests
    {
        [TestCase(null)]
        [TestCase(" ")]
        [TestCase(0)]
        public void GetValidOddsTypeNameById_OddTypeIdIsEmpty_ReturnEmptyString(object oddsTypeId)
        {
            // Arrange
            var ticketHelper = Substitute.For<ITicketHelper>();

            // Action
            var result = ticketHelper.GetValidOddsTypeNameById(oddsTypeId);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetValidOddsTypeNameById_HKOdds_ReturnCorrectString()
        {
            // Arrange
            var ticketHelper = Substitute.For<ITicketHelper>();

            // Action
            var result = ticketHelper.GetValidOddsTypeNameById(OddsType.Hongkong);

            // Assert
            Assert.That(result, Is.EqualTo(ViewResources.HKOdds));
        }

        [TestCase("3")]
        [TestCase(1)]
        [TestCase(4)]
        [TestCase("5")]
        public void GetValidOddsTypeNameById_OddTypeIdIsNotEmpty_CallGetOddsTypeNameByIdMethod(object oddsTypeId)
        {
            // Arrange
            var ticketHelper = Substitute.For<ITicketHelper>();

            // Action
            ticketHelper.GetValidOddsTypeNameById(oddsTypeId);

            // Assert
            ticketHelper.Received(1).GetOddsTypeNameById(oddsTypeId);
        }
    }
}
