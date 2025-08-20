namespace Fanex.Athena.BetList.Excel
{
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;

    public interface IExchangeRunningExcel
    {
        byte[] ExportRunningExcel(string template, ExchangeResult<ExchangeRunningTicket> data, ExchangeRunningTicketMetaModel metaModel);
    }
}
