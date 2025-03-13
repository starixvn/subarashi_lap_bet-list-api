using Newtonsoft.Json;

namespace Fanex.Athena.BetList.Model.ViewModel
{
    public class BetListSettingViewModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("baseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonProperty("fromDate")]
        public string FromDate { get; set; }

        [JsonProperty("toDate")]
        public string ToDate { get; set; }

        [JsonProperty("isServerPaging")]
        public bool IsServerPaging { get; set; }

        [JsonProperty("isAllowedCustomerCommission")]
        public bool IsAllowedCustomerCommission { get; set; }
    }
}
