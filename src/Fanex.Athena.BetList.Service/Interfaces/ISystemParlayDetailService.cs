namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Fanex.BetList.Core.Entities;
    using Model.MetaModel;

    public interface ISystemParlayDetailService
    {
        Task<string> BuildHtmlContent(SystemParlayDetailMetaModel metaModel, ITicketHelper ticketHelper, string viewPath);
    }
}