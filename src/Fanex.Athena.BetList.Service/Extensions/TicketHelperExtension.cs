namespace Fanex.Athena.BetList.Service.Extensions
{
    using Common.Constant;
    using Fanex.BetList.Core.Entities;
    using Resources;

    public static class TicketHelperExtension
    {
        public static string GetValidOddsTypeNameById(this ITicketHelper ticketHelper, object oddsTypeId)
        {
            string oddsType = oddsTypeId?.ToString().Trim();

            if (oddsType == null || oddsType == string.Empty || oddsType == "0")
            {
                return string.Empty;
            }

            if (oddsType == OddsType.Hongkong)
            {
                return ViewResources.HKOdds;
            }

            return ticketHelper.GetOddsTypeNameById(oddsTypeId);
        }
    }
}