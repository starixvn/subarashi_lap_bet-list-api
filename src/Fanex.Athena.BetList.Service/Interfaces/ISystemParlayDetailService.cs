namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Fanex.Athena.BetList.Model.MetaModel;
    using Fanex.BetList.Core.Entities;
    using System.Threading.Tasks;

    public interface ISystemParlayDetailService
    {
        Task<string> BuildHtmlContent(SystemParlayDetailMetaModel metaModel, ITicketHelper ticketHelper, string viewPath);
    }
}