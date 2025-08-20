namespace Fanex.Athena.BetList.DataAccess.Tests.TestClasses
{
    using System.Data;
    using Common.DataAccess.Interfaces;
    using Common.Model.MetaModel;
    using Common.Utility.Collections;
    using Data;
    using DataAccess.Util;
    using Fanex.BetList.Core.Entities;
    using Model.DTO;

    public class BaseBetListDataAccessTestClass : BaseBetListDataAccess
    {
        protected BaseBetListDataAccessTestClass(ICurrencyDataAccess currenyDataAccess) : base(currenyDataAccess)
        {
        }

        public new Task<IDictionary<int, string>> GetCurrencyListAsync()
            => base.GetCurrencyListAsync();

        public new SortableList<LrfTicket> ReadBetListTicket(
            IDataReader dataReader,
            IDictionary<int, string> currencyList,
            long custId = 0)
            => base.ReadBetListTicket(dataReader, currencyList, custId);

        public new ISafeDataReader GetSafeDataReader(IDataReader reader, IEnumerable<string> columns)
            => base.GetSafeDataReader(reader, columns);

        public new async Task<TicketResult> GetMainTicketListBasedRangeAsync<TMetaModel>(TMetaModel metaModel, string uspKey,
          Action<IObjectDb> assignCommonParamters,
          bool getInOneRange = false,
          Action<IObjectDb> assignShardSpecificParamters = null,
          Action<IObjectDb> assignCentralSpecificParamters = null)
          where TMetaModel : IShardDBMetaModel
            => await base.GetMainTicketListBasedRangeAsync(metaModel, uspKey, assignCommonParamters, getInOneRange, assignShardSpecificParamters, assignCentralSpecificParamters);

        public new IEnumerable<ITicketData> ReadReferneceTicket(IDataReader dataReader)
            => BaseBetListDataAccess.ReadReferneceTicket(dataReader);
    }
}
