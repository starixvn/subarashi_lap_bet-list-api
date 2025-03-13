
using Newtonsoft.Json;
using System.Reflection;

namespace Fanex.Athena.TestHelper
{
    public static class ObjectHelper
    {
        public static string ToJson(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static IDictionary<string, object?> ToDictionary(object value)
        {
            return value.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(value, null));
        }

        public static object GetValue(this object item, string property)
        {
            return item.GetType().GetProperty(property).GetValue(item, null);
        }
    }
}
