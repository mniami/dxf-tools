
namespace DxfToolLib.Helpers
{
    public interface IDxfParser
    {
        int FindHighPoints(string dxfHighPointName, string filePath, string outputPath);
        string[] FindHighPoints(int dxfVersion, string dxfHighPointName, string[] inputLines);
        int FindAllGpsCoords(string filePath, string outputPath);
    }
}