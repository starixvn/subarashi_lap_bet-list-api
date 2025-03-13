using Fanex.Athena.Common.Model.MetaModel;
using Fanex.Core.Utils;

namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class LiveReportBetListMetaModel : BetListExcelMetaModel, IAgentMetaModel, ISubsidiarySearchMetaModel
    {
        public string ProductName { get; set; }
    }
}