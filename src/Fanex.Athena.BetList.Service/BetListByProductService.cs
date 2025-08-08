namespace Fanex.Athena.BetList.Service
{
    using DataAccess.Interfaces;
    using Interfaces;
    using Model.DTO;
    using Model.MetaModel;

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
