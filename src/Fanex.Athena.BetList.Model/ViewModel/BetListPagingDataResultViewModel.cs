namespace Fanex.Athena.BetList.Model.ViewModel
{
    using Newtonsoft.Json;

    public class BetListPagingDataResultViewModel
    {
        public int Page { get; set; }

        public int Total { get; set; }

        public int Records { get; set; }

        public List<object> Rows { get; set; }

        [JsonProperty(PropertyName = "userdata")]
        public object UserData { get; set; }
    }
}
