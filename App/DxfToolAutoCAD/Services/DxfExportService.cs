#if AUTOCAD_API_AVAILABLE
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
#endif
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using DxfToolLib.Helpers;

namespace DxfToolAutoCAD
{
    /// <summary>
    /// Service for importing SoundPlan data into AutoCAD
    /// </summary>
    public class SoundPlanImportService : ISoundPlanImportService
    {
        private readonly ILogger<SoundPlanImportService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SoundPlanImportService(ILogger<SoundPlanImportService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

#if AUTOCAD_API_AVAILABLE
        /// <summary>
        /// Import SoundPlan data and add entities to the current AutoCAD document
        /// </summary>
        public int ImportSoundPlanData(string soundPlanFilePath, BlockTableRecord targetModelSpace)
        {
            return ImportSoundPlanDataWithTransform(soundPlanFilePath, targetModelSpace, null);
        }

        /// <summary>
        /// Import SoundPlan data with coordinate transformation options
        /// </summary>
        public int ImportSoundPlanDataWithTransform(string soundPlanFilePath, BlockTableRecord targetModelSpace, CoordinateTransformOptions? coordinateTransform = null)
        {
            try
            {
                _logger.LogInformation("Starting SoundPlan import from {FilePath}", soundPlanFilePath);

                if (!File.Exists(soundPlanFilePath))
                {
                    _logger.LogError("SoundPlan file not found: {FilePath}", soundPlanFilePath);
                    return 0;
                }

                // Read SoundPlan file
                var soundPlanLines = File.ReadAllLines(soundPlanFilePath, Encoding.UTF8);
                _logger.LogInformation("Read {LineCount} lines from SoundPlan file", soundPlanLines.Length);

                // Parse SoundPlan data using existing DxfToolLib logic
                var soundPlanData = ParseSoundPlanData(soundPlanLines);
                _logger.LogInformation("Parsed {DataCount} SoundPlan entries", soundPlanData.Count);

                // Apply coordinate transformation if specified
                if (coordinateTransform != null)
                {
                    ApplyCoordinateTransform(soundPlanData, coordinateTransform);
                }

                // Add entities to AutoCAD
                int entitiesAdded = 0;
                using (var transaction = targetModelSpace.Database.TransactionManager.StartTransaction())
                {
                    var modelSpace = transaction.GetObject(targetModelSpace.ObjectId, OpenMode.ForWrite) as BlockTableRecord;

                    foreach (var data in soundPlanData)
                    {
                        try
                        {
                            var entities = CreateEntitiesFromSoundPlanData(data);
                            foreach (var entity in entities)
                            {
                                modelSpace!.AppendEntity(entity);
                                transaction.AddNewlyCreatedDBObject(entity, true);
                                entitiesAdded++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to create entity for SoundPlan data index {Index}", data.Idx);
                        }
                    }

                    transaction.Commit();
                }

                _logger.LogInformation("Successfully imported {EntitiesAdded} entities from SoundPlan data", entitiesAdded);
                return entitiesAdded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing SoundPlan data from {FilePath}", soundPlanFilePath);
                return 0;
            }
        }

        /// <summary>
        /// Parse SoundPlan file lines into structured data
        /// </summary>
        private List<SoundPlanData> ParseSoundPlanData(string[] lines)
        {
            var result = new List<SoundPlanData>();
            
            // Skip header line if present
            int startIndex = lines.Length > 0 && (lines[0].Contains("Latitude") || lines[0].Contains("Idx")) ? 1 : 0;

            for (int i = startIndex; i < lines.Length; i++)
            {
                try
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 6) // Ensure we have enough columns
                    {
                        var data = new SoundPlanData
                        {
                            Idx = int.TryParse(parts[0], out int idx) ? idx : i,
                            Latitude = parts[1],
                            Longitude = parts[2],
                            Height = parts.Length > 3 ? parts[3] : "0",
                            Lrd = parts.Length > 4 ? parts[4] : "0",
                            Lrn = parts.Length > 5 ? parts[5] : "0",
                            Lrdn = parts.Length > 6 ? parts[6] : "0"
                        };
                        result.Add(data);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse SoundPlan line {LineNumber}: {Line}", i + 1, lines[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Apply coordinate transformation to SoundPlan data
        /// </summary>
        private void ApplyCoordinateTransform(List<SoundPlanData> soundPlanData, CoordinateTransformOptions transform)
        {
            foreach (var data in soundPlanData)
            {
                if (double.TryParse(data.Latitude, out double lat) && 
                    double.TryParse(data.Longitude, out double lon) &&
                    double.TryParse(data.Height, out double height))
                {
                    // Apply transformations
                    var x = (lat + transform.OffsetX) * transform.ScaleFactor;
                    var y = (lon + transform.OffsetY) * transform.ScaleFactor;
                    var z = (height + transform.OffsetZ) * transform.ScaleFactor;

                    // Apply rotation if specified
                    if (Math.Abs(transform.RotationAngle) > 1e-6)
                    {
                        var cos = Math.Cos(transform.RotationAngle);
                        var sin = Math.Sin(transform.RotationAngle);
                        var newX = x * cos - y * sin;
                        var newY = x * sin + y * cos;
                        x = newX;
                        y = newY;
                    }

                    data.Latitude = x.ToString("F6");
                    data.Longitude = y.ToString("F6");
                    data.Height = z.ToString("F6");
                }
            }
        }

        /// <summary>
        /// Create AutoCAD entities from SoundPlan data
        /// </summary>
        private List<Entity> CreateEntitiesFromSoundPlanData(SoundPlanData data)
        {
            var entities = new List<Entity>();

            if (double.TryParse(data.Latitude, out double x) && 
                double.TryParse(data.Longitude, out double y) &&
                double.TryParse(data.Height, out double z))
            {
                var point = new Point3d(x, y, z);

                // Create a point entity
                var dbPoint = new DBPoint(point);
                dbPoint.Layer = "SoundPlan_Points";
                entities.Add(dbPoint);

                // Create text labels for the sound data
                var textHeight = 2.5;
                var textOffset = 3.0;

                // Main label with Lrd value
                var mainText = new DBText();
                mainText.Position = new Point3d(point.X + textOffset, point.Y + textOffset, point.Z);
                mainText.TextString = $"Lrd: {data.Lrd}";
                mainText.Height = textHeight;
                mainText.Layer = "SoundPlan_Text";
                mainText.ColorIndex = 1; // Red
                entities.Add(mainText);

                // Additional labels for Lrn and Lrdn if they have meaningful values
                if (!string.IsNullOrWhiteSpace(data.Lrn) && data.Lrn != "0")
                {
                    var lrnText = new DBText();
                    lrnText.Position = new Point3d(point.X + textOffset, point.Y, point.Z);
                    lrnText.TextString = $"Lrn: {data.Lrn}";
                    lrnText.Height = textHeight * 0.8;
                    lrnText.Layer = "SoundPlan_Text";
                    lrnText.ColorIndex = 2; // Yellow
                    entities.Add(lrnText);
                }

                if (!string.IsNullOrWhiteSpace(data.Lrdn) && data.Lrdn != "0")
                {
                    var lrdnText = new DBText();
                    lrdnText.Position = new Point3d(point.X + textOffset, point.Y - textOffset, point.Z);
                    lrdnText.TextString = $"Lrdn: {data.Lrdn}";
                    lrdnText.Height = textHeight * 0.8;
                    lrdnText.Layer = "SoundPlan_Text";
                    lrdnText.ColorIndex = 3; // Green
                    entities.Add(lrdnText);
                }

                // Create a small circle around the point for better visibility
                var circle = new Circle();
                circle.Center = point;
                circle.Radius = 1.0;
                circle.Layer = "SoundPlan_Points";
                circle.ColorIndex = 4; // Cyan
                entities.Add(circle);
            }

            return entities;
        }
#endif
    }
}
