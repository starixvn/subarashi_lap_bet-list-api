// -----------------------------------------------------------------------
// <copyright file="LiveReportBetListMetaModel.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Fanex.Athena.Common.Model.MetaModel;
using Fanex.Core.Utils;

namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class LiveReportBetListMetaModel : BetListExcelMetaModel, IAgentMetaModel, ISubsidiarySearchMetaModel
    {
        public new int SportType { get; set; }
    }
}