
namespace DxfToolLib.Helpers
{
    public interface IDxfParser
    {
        int FindHighPoints(string filePath, string outputPath);
        int FindHighPoints(string dxfHighPointName, string filePath, string outputPath);
        string[] FindHighPoints(int dxfVersion, string dxfHighPointName, string[] inputLines);
        int FindPointsWithMultiLeadersSave(string dxfFilePath, string soundPlanFilePath, string outputPath);
        string[] FindPointsWithMultiLeaders(int dxfVersion, string[] inputLines, string[] soundPlanLines);
        int FindAllGpsCoords(string filePath, string outputPath);
    }
}