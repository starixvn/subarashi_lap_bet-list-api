using Fanex.Result.Core.ThirdParty;

namespace Fanex.Athena.Result.Service.Interfaces
{
    public interface IBetlistResultGetterFactory
    {
        IThirdPartyResult CreateThirdPartyResultGetter(string configName);
    }
}
