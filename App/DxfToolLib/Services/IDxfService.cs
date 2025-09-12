using DxfToolLib.Models;

namespace DxfToolLib.Helpers
{
    public interface IDxfService
    {
        int FindHighPoints(string filePath, string outputPath);
        int FindHighPoints(string dxfHighPointName, string filePath, string outputPath);
        string[] FindHighPoints(int dxfVersion, string dxfHighPointName, string[] inputLines);
        int FindPointsWithMultiLeadersSave(string dxfFilePath, string soundPlanFilePath, string finalTableCsvFilePath, string outputPath);
        string[] FindPointsWithMultiLeaders(int dxfVersion, string[] inputLines, string[] soundPlanLines);
        string[] FindPointsWithMultiLeaders(int dxfVersion, string[] inputLines, string[] soundPlanLines, FinalTableData[] finalTableData);
        int FindAllGpsCoords(string filePath, string outputPath);
    }
}