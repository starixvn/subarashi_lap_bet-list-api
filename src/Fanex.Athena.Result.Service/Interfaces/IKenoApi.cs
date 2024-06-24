namespace Fanex.Athena.Result.Service.Interfaces
{
    using System;
    using Fanex.Athena.Result.Model.MetaModel.Keno;
    using Model.DTO.Keno;

    public interface IKenoApi
    {
        Task<KenoExtendedToken> ExtendToken(KenoResultMetaModel setting, string userId);

        Task<string> CreateKenoResultAddress(KenoResultMetaModel setting, string userId, DateTime resultDate);
    }
}