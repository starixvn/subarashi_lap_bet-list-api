namespace Fanex.Athena.Result.Service.Interfaces
{
    using Fanex.Result.Core.KenoLotteryTG;

    public interface IKenoLotteryTGResultFactory
    {
        IKenoLotteryTGResult CreateResultApi(int vendorProductId);
    }
}
