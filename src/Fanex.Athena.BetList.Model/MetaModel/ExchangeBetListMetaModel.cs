namespace Fanex.Athena.BetList.Model.MetaModel
{
    using System;

    public class ExchangeBetListMetaModel : BaseExchangeBetListMetaModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public byte DateOptionType { get; set; } = DateOptionTypes.SettledAndResettledReport;

        public bool IsViewResettledTicketOnly { get; set; }

        public override bool IsValid => base.IsValid && FromDate <= ToDate;
    }
}