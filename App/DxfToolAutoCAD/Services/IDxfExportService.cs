#if AUTOCAD_API_AVAILABLE
using Autodesk.AutoCAD.DatabaseServices;
#endif

namespace DxfToolAutoCAD
{
    /// <summary>
    /// Interface for SoundPlan import services
    /// </summary>
    public interface ISoundPlanImportService
    {
#if AUTOCAD_API_AVAILABLE
        /// <summary>
        /// Import SoundPlan data and add entities to the current AutoCAD document
        /// </summary>
        /// <param name="soundPlanFilePath">Path to the SoundPlan file</param>
        /// <param name="targetModelSpace">The model space to add entities to</param>
        /// <returns>Number of entities added</returns>
        int ImportSoundPlanData(string soundPlanFilePath, BlockTableRecord targetModelSpace);
        
        /// <summary>
        /// Import SoundPlan data with coordinate transformation options
        /// </summary>
        /// <param name="soundPlanFilePath">Path to the SoundPlan file</param>
        /// <param name="targetModelSpace">The model space to add entities to</param>
        /// <param name="coordinateTransform">Optional coordinate transformation settings</param>
        /// <returns>Number of entities added</returns>
        int ImportSoundPlanDataWithTransform(string soundPlanFilePath, BlockTableRecord targetModelSpace, CoordinateTransformOptions? coordinateTransform = null);
#endif
    }

    /// <summary>
    /// Coordinate transformation options for import
    /// </summary>
    public class CoordinateTransformOptions
    {
        public double OffsetX { get; set; } = 0;
        public double OffsetY { get; set; } = 0;
        public double OffsetZ { get; set; } = 0;
        public double ScaleFactor { get; set; } = 1.0;
        public double RotationAngle { get; set; } = 0; // in radians
    }
}
