namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class RunningBetListMetaModel : BaseBettingTicketMetaModel
    {
        public string LiveStatus { get; set; }

        public string SportIds { get; set; }

        public string SportListXML { get; set; }

        public string BetTypeIds { get; set; }

        public string LeagueIds { get; set; }

        public int MatchId { get; set; }

        public string Ip { get; set; }

        public string OddsGroup { get; set; }

        public string Platform { get; set; }

        public string CustomerClass { get; set; }

        public string FullCustomerClass { get; set; }
    }
}