using Fanex.Athena.Result.Controller.ResultService.Interfaces;
using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.ThirdParty.Models;

namespace Fanex.Athena.Result.Controller.ResultService
{
    public class ThirdPartySportResultService : IThirdPartySportResultService
    {
        private readonly IBetlistResultGetterFactory _factory;

        public ThirdPartySportResultService(IBetlistResultGetterFactory factory)
            => _factory = factory;

        public string GetResultUrl(ResultPopupMetaModel metaModel, string configKey)
        {
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(configKey));
            var gameId3rd = metaModel.Extension2;
            var betTypeId = Int32.TryParse(metaModel.Extension1, out int parsedBetTypeId) ? parsedBetTypeId : 0;

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, gameId3rd, betTypeId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.Bettype
            };

            return resultApi.GetResultGET(request);
        }
    }
}