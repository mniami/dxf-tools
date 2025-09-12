using DxfToolLib.Models;

namespace DxfToolLib.Services
{
    public interface IDxfService
    {
        int FindHighPoints(string filePath, string outputPath);
        int FindHighPoints(string dxfHighPointName, string filePath, string outputPath);
        string[] FindHighPoints(int dxfVersion, string dxfHighPointName, string[] inputLines);
        string[] FindHighPoints(int dxfVersion, string[] inputLines);
        int FindAllGpsCoords(string filePath, string outputPath);
        int UpdateDxfPointsWithSoundPlanDataSave(string dxfFilePath, string soundPlanFilePath, string finalTableCsvFilePath, string outputPath);
        DxfPoint[] UpdateDxfPointsWithSoundPlanData(string[] dxfInputLines, SoundPlanPoint[] soundPlanPoints, FinalTableData[] finalTableData);
    }
}