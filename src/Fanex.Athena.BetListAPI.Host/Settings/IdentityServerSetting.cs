namespace Fanex.Athena.BetListAPI.Site.Settings
{
    public class IdentityServerSetting
    {
        public string Host { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Scope { get; set; }

        public string HealthPath { get; set; }

        public int CacheDuration { get; set; }
    }
}
