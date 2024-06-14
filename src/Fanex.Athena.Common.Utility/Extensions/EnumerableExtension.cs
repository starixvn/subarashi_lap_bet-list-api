

namespace Fanex.Athena.Common.Utility.Extensions
{
    public static class EnumerableExtension
    {
        public static bool IsPresent<T>(this IEnumerable<T> source)
            => source != null && source.Any();

        public static bool IsPresent<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => source != null && source.Any(predicate);
    }
}
