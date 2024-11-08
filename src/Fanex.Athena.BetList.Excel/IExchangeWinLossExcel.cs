using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;

namespace Fanex.Athena.BetList.Excel
{
    public interface IExchangeWinLossExcel
    {
        byte[] ExportExcel(string template, ExchangeBetListViewModel data, ExchangeBetListMetaModel metaModel);
    }
}
