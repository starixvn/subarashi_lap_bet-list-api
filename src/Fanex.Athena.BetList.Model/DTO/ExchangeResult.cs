using System.Collections.Generic;


namespace Fanex.Athena.BetList.Model.DTO
{
    public class ExchangeResult<T> where T : class
    {
        public IEnumerable<T> DataList { get; set; }

        public int TotalRows { get; set; }

        public decimal SumStake { get; set; }
    }
}
