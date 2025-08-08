namespace Fanex.Athena.BetList.Model.MetaModel
{
    using System.ComponentModel;

    [Serializable]
    public class BaseMetaModel
    {
        private string _baseCurrency = string.Empty;

        public string AgentListXml { get; set; }

        public int LicenseeId { get; set; }

        public int UserId { get; set; }

        public int CurrencyId { get; set; }

        [DefaultValue("")]
        public string CurrencyName { get; set; }

        [DefaultValue("")]
        public string BaseCurrency
        {
            get
            {
                return _baseCurrency;
            }

            set
            {
                _baseCurrency = value == null
                    ? string.Empty
                    : value.ToLowerInvariant();
            }
        }
    }
}