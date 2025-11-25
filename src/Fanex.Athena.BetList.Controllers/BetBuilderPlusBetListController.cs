namespace Fanex.Athena.BetList.Controllers
{
    using Fanex.BetList.Core.Builder.HybridParlayBuilder;
    using Fanex.BetList.Core.Entities;
    using Microsoft.AspNetCore.Mvc;
    using Model.DTO;
    using Model.MetaModel;
    using Service.Helpers;
    using Service.Interfaces;

    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BetBuilderPlusBetListController
    {
        private readonly IBetListService _betListService;

        private readonly ITicketHelper _ticketHelper;

        private readonly IHybridParlayBuilder _hybridParlayBuilder;

        private readonly IIPService _ipService;

        public BetBuilderPlusBetListController(
            IBetListService betListService,
            ITicketHelper ticketHelper,
            IHybridParlayBuilder hybridParlayBuilder,
             IIPService ipService)
        {
            _betListService = betListService;
            _ticketHelper = ticketHelper;
            _hybridParlayBuilder = hybridParlayBuilder;
            _ipService = ipService;
        }

        [HttpPost]
        public async Task<object> Detail(BetBuilderPlusDetailMetaModel metaModel)
        {
            var ticketResult = await _betListService.GetMixParlayDetailTicket(metaModel);

            BuildTicketHelper(true, metaModel.IsShowIp, metaModel.IsHideSaba);

            var result = new
            {
                Data = BuildParlayDetailTickets(ticketResult, metaModel)
            };

            BetListHelper.ClearData(ticketResult);
            return result;
        }

        private void BuildTicketHelper(bool showOdds = false, bool isShowIp = false, bool isHideSaba = false)
        {
            _ticketHelper.IsShowOddsWithChoice = showOdds;

            _ticketHelper.HideSabaText = isHideSaba;
            _ticketHelper.IsVnIp = _ipService.IsAlwaysIpvn;

            // If show ip check ipvn
            if (isShowIp)
            {
                _ticketHelper.IsVnIp = _ipService.IsVNIP;
            }
        }

        protected internal virtual string BuildParlayDetailTickets(TicketResult ticketResult, BetBuilderPlusDetailMetaModel metaModel)
        {
            var ticketList = ticketResult.MainTicketList.Cast<ITicket>().ToList();

            var ticket = ticketList.FirstOrDefault();

            var result = _hybridParlayBuilder.Build(
                _ticketHelper,
                ticketList,
                metaModel.TransDesc,
                (int)metaModel.Handicap1,
                (int)metaModel.Handicap2,
                metaModel.BetCheck,
                metaModel.BetTeam,
                metaModel.Currency,
                ticket.Stake,
                false);

            return result;
        }
    }
}