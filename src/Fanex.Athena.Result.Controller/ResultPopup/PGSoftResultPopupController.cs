using Fanex.Athena.Result.Controller.MetaModel;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Mvc;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Result.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PGSoftResultPopupController
    {
        private readonly IIpInfoService _IpInfoService;
        private readonly IBetlistResultGetterFactory _factory;
        private readonly IHttpContextAccessor _contextAccessor;

        public PGSoftResultPopupController(IIpInfoService IpInfoService, IBetlistResultGetterFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _IpInfoService = IpInfoService;
            _factory = factory;
            _contextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public string Index(ThirdPartyMetaModel metaModel)
        {
            var resultAPI = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.PGSoftResult));
            var clientIp = string.IsNullOrEmpty(metaModel.ClientIp) ? _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString() : metaModel.ClientIp;

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId,
                Location = _IpInfoService.GetIpLocation(clientIp)
            };

            return resultAPI.GetResultGET(request);
        }
    }
}
