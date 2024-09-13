using System;

namespace Fanex.Athena.BetList.Model.DTO
{
    public class ExchangeRunningTicket
    {
        public long BetId { get; set; }

        public string UserId { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }

        public string NickName { get; set; }

        public int CategoryType { get; set; }

        public string EventTypeName { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; }

        public string MarketId { get; set; }

        public string MarketName { get; set; }

        public int SelectionId { get; set; }

        public string SelectionName { get; set; }

        public string Type { get; set; }

        public DateTime BetPlaced { get; set; }

        public int Currency { get; set; }

        public decimal OddsAvg { get; set; }

        public decimal FancyBetRuns { get; set; }

        public decimal Stake { get; set; }

        public string BetPlacedString => BetPlaced.ToString("MM/dd/yyyy hh:mm:ss tt");
    }
}
