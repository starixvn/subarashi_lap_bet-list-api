using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.TestHelper.Extensions
{
    public static class NSubstitueExtensions
    {
        public static object SubstituteProtectedMethod<T>(this T obj, string methodName, params object[] parameters)
        {
            var type = obj.GetType();
            var method = type.GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return method.Invoke(obj, parameters);
        }
    }
}
