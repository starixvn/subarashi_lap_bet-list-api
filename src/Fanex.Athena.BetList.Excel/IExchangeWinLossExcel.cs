namespace Fanex.Athena.BetList.Excel
{
    using BetList.Model.MetaModel;
    using BetList.Model.ViewModel;

    public interface IExchangeWinLossExcel
    {
        byte[] ExportExcel(string template, ExchangeBetListViewModel data, ExchangeBetListMetaModel metaModel);
    }
}
