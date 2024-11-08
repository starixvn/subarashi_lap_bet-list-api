namespace Fanex.Athena.BetList.Controllers
{
    using Fanex.Athena.BetList.Controllers.CustomActionResult;
    using Fanex.Athena.BetList.Excel;
    using Fanex.Athena.BetList.Excel.Model;
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using Fanex.Athena.BetList.Service.Helpers;
    using Fanex.Athena.BetList.Service.Interfaces;
    using Fanex.Athena.Common.Constant;
    using Fanex.Athena.Resources;
    using Microsoft.AspNetCore.Mvc;
    using System;

    public class BettingTicketCenterBaseController : ControllerBase
    {
        private readonly IBettingTicketCenterExcel _bettingTicketCenterExcel;

        private readonly ITicketBuilderService _ticketBuilderService;

        public BettingTicketCenterBaseController(
            ITicketBuilderService ticketBuilderService,
            IBettingTicketCenterExcel bettingTicketCenterExcel)
        {
            _ticketBuilderService = ticketBuilderService;
            _bettingTicketCenterExcel = bettingTicketCenterExcel;
        }

        protected static string GetTitle(BaseBettingTicketMetaModel metaModel)
        {
            if (metaModel.ViewSabaCoins)
            {
                return $"{ViewResources.BettingTicketCenter} {string.Format(ViewResources.Viewin, ViewResources.SabaCoins)}";
            }

            var date = metaModel.FromDate == DateTimeOffset.MinValue.DateTime
                ? string.Empty
                : $", {metaModel.FromDate.ToString(FormatKey.DateFormat)}-->{metaModel.ToDate.ToString(FormatKey.DateFormat)}";

            var baseCurrencyName = metaModel.IsConvertAll ? metaModel.BaseCurrencyName : string.Empty;
            var baseCurrencyText = !string.IsNullOrEmpty(metaModel.BaseCurrency) ? metaModel.BaseCurrency.ToUpperInvariant() : baseCurrencyName;

            var baseCurrency = string.IsNullOrEmpty(baseCurrencyText)
                ? string.Empty
                : $", {ViewResources.BaseCurrency}: {baseCurrencyText}";

            return ViewResources.BettingTicketCenter + date + (metaModel.IsConvertAll ? baseCurrency : string.Empty);
        }

        protected internal virtual ExcelResult ExportExcel(BettingTicketCenterExcelModel excelModel)
        {
            var workbook = _bettingTicketCenterExcel.Export(excelModel);
            BetListHelper.ClearData(excelModel.Data);
            excelModel.Data = null;
            return new ExcelResult(workbook, "BettingTicketCenter");
        }

        protected internal virtual object ReturnData(TicketResult ticket, BettingTicketCenterOption option, ICommonValueMetaModel metaModel)
        {
            var result = new
            {
                Data = option.IsBookedBet
                    ? _ticketBuilderService.BuildBookedBetTicketText(ticket, metaModel)
                    : _ticketBuilderService.BuildTicketText(ticket, metaModel, option.IsAllowedCustomerComm),
                Total = new
                {
                    WinLoss = (option.ShowTotal && option.ShowWinLoss) ? _ticketBuilderService.BuildSumWinLoss(ticket, option.IsAllowedCustomerComm) : null,
                    LicenseeWL = (option.ShowTotal && option.ShowLicenseeWl) ? (ticket.SumLicenseeWL + ticket.SumComm).ToString(FormatKey.DecimalFormat) : null,
                    Stake = option.ShowSumStake ? ticket.SumStake.ToString(FormatKey.DecimalFormat) : null
                },
                ticket.TotalRows
            };

            BetListHelper.ClearData(ticket);
            return result;
        }
    }
}