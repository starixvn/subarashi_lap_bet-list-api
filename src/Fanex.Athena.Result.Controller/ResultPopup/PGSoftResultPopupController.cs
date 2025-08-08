namespace Fanex.Athena.Result.Controller.ResultPopup
{
    using BetList.Service.Interfaces;
    using Fanex.Result.Core.ThirdParty.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Interfaces;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class PGSoftResultPopupController
    {
        private readonly IIpInfoService _ipInfoService;
        private readonly IBetlistResultGetterFactory _factory;
        private readonly IHttpContextAccessor _contextAccessor;

        public PGSoftResultPopupController(IIpInfoService IpInfoService, IBetlistResultGetterFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _ipInfoService = IpInfoService;
            _factory = factory;
            _contextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public IActionResult Index(ThirdPartyMetaModel metaModel)
        {
            var resultAPI = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.PGSoftResult));
            var clientIp = string.IsNullOrEmpty(metaModel.ClientIp) ? _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString() : metaModel.ClientIp;

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId,
                Location = _ipInfoService.GetIpLocation(clientIp)
            };

            return new ContentResult { Content = resultAPI.GetResultGET(request) };
        }
    }
}
