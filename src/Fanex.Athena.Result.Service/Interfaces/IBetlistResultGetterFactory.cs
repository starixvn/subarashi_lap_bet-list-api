namespace Fanex.Athena.Result.Service.Interfaces
{
    using Fanex.Result.Core.ThirdParty;

    public interface IBetlistResultGetterFactory
    {
        IThirdPartyResult CreateThirdPartyResultGetter(string configName);
    }
}
