namespace Fanex.Athena.BetList.Service
{
    using Fanex.Athena.BetList.DataAccess.Interfaces;
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using Fanex.Athena.BetList.Service.Interfaces;
    using System.Threading.Tasks;

    public class BetListByProductService : IBetListByProductService
    {
        private readonly IBetListByProductDataAccess _betListByProductDataAccess;

        public BetListByProductService(IBetListByProductDataAccess betListByProductDataAccess)
        {
            _betListByProductDataAccess = betListByProductDataAccess;
        }

        public async Task<TicketResult> QueryAsync(BetListByProductMetaModel metaModel)
        {
            return await _betListByProductDataAccess.QueryAsync(metaModel);
        }
    }
}
