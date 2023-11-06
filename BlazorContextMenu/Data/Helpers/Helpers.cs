using System.Linq;

namespace BlazorContextMenu;

internal static class Helpers
{
    public static string AppendCssClasses(params string[] cssClasses)
    {
        return cssClasses.Where(StringExtensions.HasValue).Merge(" ");
    }
}