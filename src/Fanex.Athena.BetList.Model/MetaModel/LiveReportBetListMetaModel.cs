namespace Fanex.Athena.BetList.Model.MetaModel
{
    using Common.Model.MetaModel;
    using Core.Utils;

    public class LiveReportBetListMetaModel : BetListExcelMetaModel, IAgentMetaModel, ISubsidiarySearchMetaModel
    {
        public string ProductName { get; set; }

        public string VendorMatchId { get; set; }

        public string Distance { get; set; }
    }
}