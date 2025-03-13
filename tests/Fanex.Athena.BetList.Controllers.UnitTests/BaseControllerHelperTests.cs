using Fanex.Athena.BetList.Model.DTO;
using NUnit.Framework;

namespace Fanex.Athena.BetList.Controllers.UnitTests
{
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
