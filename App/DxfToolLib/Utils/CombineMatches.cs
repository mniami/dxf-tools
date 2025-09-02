public static class StringArrayExtensions
{
    public static string[] CombineMatches(this IEnumerable<string[]> strings)
    {
        return [.. strings.Select(item => string.Join(",", item))];
    }
}