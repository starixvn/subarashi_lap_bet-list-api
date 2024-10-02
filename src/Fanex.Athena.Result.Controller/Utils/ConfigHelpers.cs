namespace Fanex.Athena.Result.Controller.Utils
{
    internal static class ConfigHelpers
    {
        public static string GetConfigKey(string configName)
        {
            return $"{ConfigName.ConfigPrefix}:{configName}";
        }
    }
}
