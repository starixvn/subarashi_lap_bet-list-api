using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;

namespace Fanex.Athena.BetList.Excel
{
    public interface IExchangeRunningExcel
    {
        byte[] ExportRunningExcel(string template, ExchangeResult<ExchangeRunningTicket> data, ExchangeRunningTicketMetaModel metaModel);
    }
}
