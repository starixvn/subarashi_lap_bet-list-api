namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class BetListExcelMetaModel : BetListMetaModel
    {
        public string Title { get; set; }

        public BetListShowingOption ShowingOption { get; set; }

        public int[] HiddenColumns { get; set; }

        public int StartIndex { get; set; } = 1;

        public string ProductCode { get; set; }

        public string MatchName { get; set; }

        public bool IsNeutral { get; set; }
    }
}