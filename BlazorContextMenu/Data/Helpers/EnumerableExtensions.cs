using System.Collections.Generic;

namespace BlazorContextMenu;

internal static class EnumerableExtensions
{
    public static string Merge(this IEnumerable<string> enumerable, string separator)
    {
        return string.Join(separator, enumerable);
    }
}