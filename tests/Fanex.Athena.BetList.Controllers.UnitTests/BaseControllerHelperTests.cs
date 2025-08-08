namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    using Model.DTO;
    using NUnit.Framework;

    public class BaseControllerHelperTests
    {
        protected void AssertClearData(TicketResult ticketResult)
        {
            Assert.Multiple(() =>
            {
                Assert.That(ticketResult.MainTicketList, Is.Null);
                Assert.That(ticketResult.TicketDataList, Is.Null);
            });
        }
    }
}
