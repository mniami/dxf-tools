
namespace DxfToolLib.Helpers
{
    public interface IDxfParser
    {
        int Parse(string dxfHighPointName, string filePath, string outputPath);
        string[] Parse(string dxfHighPointName, string[] inputLines);
    }
}