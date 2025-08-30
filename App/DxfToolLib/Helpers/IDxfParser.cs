
namespace DxfToolLib.Helpers
{
    public interface IDxfParser
    {
        int FindHighPoints(string filePath, string outputPath);
        int FindHighPoints(string dxfHighPointName, string filePath, string outputPath);
        string[] FindHighPoints(int dxfVersion, string dxfHighPointName, string[] inputLines);
        int FindGeometryPoints(string filePath, string outputPath);
        string[] FindGeometryPoints(int dxfVersion, string[] inputLines);
        int FindAllGpsCoords(string filePath, string outputPath);
    }
}