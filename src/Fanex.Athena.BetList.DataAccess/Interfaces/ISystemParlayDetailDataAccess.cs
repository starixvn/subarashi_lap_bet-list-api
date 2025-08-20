namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface ISystemParlayDetailDataAccess
    {
        Task<SystemParlayDetailData> GetData(SystemParlayDetailMetaModel metaModel);
    }
}