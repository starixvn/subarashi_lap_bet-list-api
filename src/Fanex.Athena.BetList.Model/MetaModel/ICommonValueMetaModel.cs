namespace Fanex.Athena.BetList.Model.MetaModel
{
    using Common.Model.MetaModel;

    public interface ICommonValueMetaModel : IMetaModelShowBetFromIP
    {
        string BaseCurrency { get; set; }

        int PageIndex { get; set; }

        int PageSize { get; set; }

        int CustId { get; set; }

        int SpecialPlatform { get; set; }
    }
}
