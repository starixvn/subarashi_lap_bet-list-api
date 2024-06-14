using Fanex.Athena.Common.Model.MetaModel;

namespace Fanex.Athena.BetList.Model.MetaModel
{
    public interface ICommonValueMetaModel : IMetaModelShowBetFromIP
    {
        string BaseCurrency { get; set; }

        int PageIndex { get; set; }

        int PageSize { get; set; }

        int CustId { get; set; }

        int SpecialPlatform { get; set; }
    }
}
