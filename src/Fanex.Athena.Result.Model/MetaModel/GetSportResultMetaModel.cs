namespace Fanex.Athena.Result.Model.MetaModel
{
    using System;
    using System.Text.Json.Serialization;

    [Serializable]
    public class GetSportResultMetaModel : ResultMetaModel
    {
        [JsonIgnore]
        public string ProductCodeList { get; set; }
    }
}