using System;

namespace DxfTool.Models
{
    public class UserPreferences
    {
        public string DxfFilePath { get; set; } = string.Empty;
        public string SoundPlanFilePath { get; set; } = string.Empty;
        public string FinalTableCsvFilePath { get; set; } = string.Empty;
        public string DestinationFilePath { get; set; } = string.Empty;
        public DataExtractionType SelectedDataType { get; set; } = DataExtractionType.GeometryPoints;
        public DateTime LastSaved { get; set; } = DateTime.Now;
    }
}
