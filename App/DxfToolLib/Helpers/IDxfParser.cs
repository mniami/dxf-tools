
namespace DxfToolLib.Helpers
{
    public interface IDxfParser
    {
        int FindHighPoints(string dxfHighPointName, string filePath, string outputPath);
        string[] FindHighPoints(string dxfHighPointName, string[] inputLines);
        int FindAllGpsCoords(string dxfHighPointName, string filePath, string outputPath);
    }
}