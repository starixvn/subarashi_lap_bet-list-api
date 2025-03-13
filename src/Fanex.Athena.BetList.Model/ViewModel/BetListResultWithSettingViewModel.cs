using Newtonsoft.Json;

namespace Fanex.Athena.BetList.Model.ViewModel
{
    public class BetListResultWithSettingViewModel<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("settings")]
        public BetListSettingViewModel Settings { get; set; }
    }
}
