namespace Fanex.Athena.BetList.Model.ViewModel
{
    using Newtonsoft.Json;

    public class BetListResultWithSettingViewModel<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("settings")]
        public BetListSettingViewModel Settings { get; set; }
    }
}
