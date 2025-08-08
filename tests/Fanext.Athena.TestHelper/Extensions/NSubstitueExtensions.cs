namespace Fanex.Athena.TestHelper.Extensions
{
    using System.Reflection;

    public static class NSubstitueExtensions
    {
        public static object InvokeProtectedMethod<T>(this T obj, string methodName, params object[] parameters)
        {
            var type = obj.GetType();
            var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            return method.Invoke(obj, parameters);
        }

        public static Task<TResult> InvokeAsyncProtectedMethod<T, TResult>(this T obj, string methodName, params object[] parameters)
        {
            var type = obj.GetType();
            var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            return method.Invoke(obj, parameters) as Task<TResult>;
        }
    }
}
