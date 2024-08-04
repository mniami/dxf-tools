
namespace DxfToolLib.Helpers
{
    public interface IDxfParser
    {
        string[][] GetFoundMatches(IList<string> schemaItems, string[] input);
        int Parse(string dxfHighPointName, string filePath, string outputPath);
        string[] Parse(string dxfHighPointName, string[] inputLines);
    }
}