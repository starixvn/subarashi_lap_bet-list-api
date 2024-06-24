namespace Fanex.Athena.Result.Service
{
    using System;
    using System.Net;
    using System.Text;
    using System.Web;
    using Interfaces;
    using Model.Const;
    using Model.DTO.Keno;
    using Model.MetaModel.Keno;
    using Newtonsoft.Json;

    public class KenoApi : IKenoApi
    {
        private const string KENOSERVICENAME = "KenoApi";
        private const string KENOGAMENAME = "Keno";
        private const string LOTTERYSERVICENAME = "LotteryApi";
        private const string LOTTERYGAMENAME = "Lottery";
        private const string TABLESERVICENAME = "TableApi";
        private const string TABLEGAMENAME = "Table";

        private readonly HttpClient _webClient;

        public KenoApi(HttpClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<KenoExtendedToken> ExtendToken(KenoResultMetaModel setting, string userId)
        {
            try
            {
                var extendedUrl = string.Format(setting.ExtendTokenUrl, setting.Vendor, setting.VenderKey, userId, setting.VendorProduct);
                var data = await _webClient.GetStringAsync(extendedUrl);
                if (string.IsNullOrEmpty(data))
                {
                    throw new WebException(GetServiceName(setting.VendorProduct) + "Couldn't get authorized data from: " + extendedUrl);
                }

                return JsonConvert.DeserializeObject<KenoExtendedToken>(data);
            }
            catch (Exception exception)
            {
                throw new WebException(GetServiceName(setting.VendorProduct) + exception.Message);
            }
        }

        public async Task<string> CreateKenoResultAddress(KenoResultMetaModel setting, string userId, DateTime resultDate)
        {
            var authorizedData = await Authorize(setting, userId);
            if (authorizedData.ErrorCode > 0)
            {
                throw new WebException(GetServiceName(setting.VendorProduct) + authorizedData.ErrorMsg);
            }

            var kenoResultUrl = new StringBuilder(setting.ResultUrl);
            kenoResultUrl.Append(setting.ResultUrl.Contains("?") ? "&" : "?");
            kenoResultUrl.Append("Authorization=" + authorizedData.Data.Authorization);
            kenoResultUrl.Append("&cust=" + GetGameName(setting.VendorProduct));
            kenoResultUrl.Append("&siteID=" + setting.SiteID);
            kenoResultUrl.Append("&action=R");
            kenoResultUrl.Append("&lang=" + setting.Language);
            kenoResultUrl.Append("&date=" + resultDate.ToString("yyyyMMdd"));
            kenoResultUrl.Append("&redirectURL=" + HttpUtility.UrlEncode(setting.KickOutUrl));
            return kenoResultUrl.ToString();
        }

        private static string GetServiceName(int vendorProductId)
        {
            string serviceName = "";

            switch (vendorProductId)
            {
                case ResultVendorProductTypes.Lottery:
                    serviceName = LOTTERYSERVICENAME;
                    break;
                case ResultVendorProductTypes.Table:
                    serviceName = TABLESERVICENAME;
                    break;
                default:
                    serviceName = KENOSERVICENAME;
                    break;
            }

            return serviceName;
        }

        private static string GetGameName(int vendorProductId)
        {
            string gameName = "";

            switch (vendorProductId)
            {
                case ResultVendorProductTypes.Lottery:
                    gameName = LOTTERYGAMENAME;
                    break;
                case ResultVendorProductTypes.Table:
                    gameName = TABLEGAMENAME;
                    break;
                default:
                    gameName = KENOGAMENAME;
                    break;
            }

            return gameName;
        }

        protected internal virtual async Task<KenoAuthorization> Authorize(KenoResultMetaModel setting, string userId)
        {
            try
            {
                var authorizedUrl = string.Format(setting.AuthorizedUrl, setting.Vendor, setting.VenderKey, userId, setting.VendorProduct);
                var data = await _webClient.GetStringAsync(authorizedUrl);
                if (string.IsNullOrEmpty(data))
                {
                    throw new WebException(GetServiceName(setting.VendorProduct) + "Couldn't get authorized data from: " + authorizedUrl, WebExceptionStatus.ReceiveFailure);
                }

                return JsonConvert.DeserializeObject<KenoAuthorization>(data);
            }
            catch (Exception exception)
            {
                throw new WebException(GetServiceName(setting.VendorProduct) + exception.Message, exception);
            }
        }
    }
}