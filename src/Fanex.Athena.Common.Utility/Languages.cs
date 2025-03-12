namespace Fanex.Athena.Common.Utility
{
    public static class Languages
    {
        public static readonly IReadOnlyDictionary<string, string> LanguageCodeMappers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "en", "en-US" },
            { "cs", "zh-CN" },
            { "vn", "vi-VN" },
            { "th", "th-TH" },
            { "id", "id-ID" },
            { "en-US", "en-US" },
            { "zh-CN", "zh-CN" },
            { "vi-VN", "vi-VN" },
            { "th-TH", "th-TH" },
            { "id-ID", "id-ID" }
        };
    }
}