public static class LoadFileExtensions
{
    public static string GetFullPath(this string filePath)
    {
        var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", filePath);
        return fullPath;
    }
}