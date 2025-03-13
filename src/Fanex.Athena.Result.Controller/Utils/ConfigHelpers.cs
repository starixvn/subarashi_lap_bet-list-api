namespace Fanex.Athena.Result.Controller.Utils
{
    internal static class ConfigHelpers
    {
        internal static string GetConfigKey(string configName)
        {
            return $"{ConfigName.ConfigPrefix}:{configName}";
        }
    }
}
