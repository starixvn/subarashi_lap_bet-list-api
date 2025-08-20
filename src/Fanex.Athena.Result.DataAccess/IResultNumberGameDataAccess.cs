namespace Fanex.Athena.Result.DataAccess
{
    using Fanex.Result.Core.Happy5.DTO;
    using Fanex.Result.Core.NumberGame.DTO;
    using Model.MetaModel;

    public interface IResultNumberGameDataAccess
    {
        Task<IList<NumberGame>> GetNumberGameMatchesAsync(ResultMetaModel metalModel);

        Task<IList<Happy5>> GetHappy5MatchesAsync(ResultMetaModel metalModel);

        Task<IList<NumberGame>> GetPopUpNumberGameMatchAsync(int matchId);

        Task<IList<Happy5>> GetPopUpHappy5MatchAsync(int matchId);
    }
}