namespace Fanex.Athena.Result.Service.Interfaces
{
    using Fanex.Athena.Result.Model.MetaModel.Keno;
    using Model.DTO.Keno;
    using System;

    public interface IKenoApi
    {
        Task<KenoExtendedToken> ExtendToken(KenoResultMetaModel setting, string userId);

        Task<string> CreateKenoResultAddress(KenoResultMetaModel setting, string userId, DateTime resultDate);
    }
}