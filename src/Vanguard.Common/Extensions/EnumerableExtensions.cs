namespace Vanguard.Common.Extensions
{
    /// <summary>
    /// Extension methods for enumerables
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Checks if a collection is null or empty
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="source">The collection to check</param>
        /// <returns>True if the collection is null or empty, false otherwise</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Returns a collection as a read-only list
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="source">The collection to convert</param>
        /// <returns>A read-only list</returns>
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return Array.Empty<T>();

            if (source is IReadOnlyList<T> readOnlyList)
                return readOnlyList;

            return source.ToList().AsReadOnly();
        }
    }
}
