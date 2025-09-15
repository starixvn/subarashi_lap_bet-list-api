namespace Fanex.Athena.Result.Controller.ResultService
{
    using Fanex.Result.Core.ThirdParty.Models;
    using Interfaces;
    using Model.MetaModel;
    using Service.Interfaces;

    public class ThirdPartySportResultService : IThirdPartySportResultService
    {
        private readonly IBetlistResultGetterFactory _factory;

        public ThirdPartySportResultService(IBetlistResultGetterFactory factory)
            => _factory = factory;

        public Uri GetResultUrl(ResultPopupMetaModel metaModel, string configKey)
        {
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(configKey));
            var gameId3rd = metaModel.Extension2;
            var betTypeId = Int32.TryParse(metaModel.Extension1, out int parsedBetTypeId) ? parsedBetTypeId : 0;

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, gameId3rd, betTypeId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.Bettype,
                EndTime = metaModel.WinLossDate.ToString()
            };

            var url = resultApi.GetResultGET(request);
            return new Uri(url);
        }
    }
}