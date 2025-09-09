#if AUTOCAD_API_AVAILABLE
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
#endif
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using DxfToolLib.Helpers;
using DxfToolLib.Models;
using System.Text;

namespace DxfToolAutoCAD.Services
{
    /// <summary>
    /// Service for importing SoundPlan data into AutoCAD
    /// </summary>
    public class SoundPlanImportService : ISoundPlanImportService
    {
        private readonly ILogger<SoundPlanImportService> _logger;
        private readonly IDxfParser _dxfParser;

        public SoundPlanImportService(ILogger<SoundPlanImportService> logger, IDxfParser dxfParser)
        {
            _logger = logger;
            _dxfParser = dxfParser;
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

                // Parse SoundPlan data using existing DxfToolLib functionality
                // This is where you'll integrate with your existing parser
                var soundPlanData = ParseSoundPlanFile(soundPlanLines);
                _logger.LogInformation("Parsed {DataCount} SoundPlan data points", soundPlanData.Count);

                int entitiesAdded = 0;

                // Create AutoCAD entities for each SoundPlan data point
                foreach (var dataPoint in soundPlanData)
                {
                    var entities = CreateAutoCADEntitiesFromSoundPlanData(dataPoint, coordinateTransform);
                    
                    foreach (var entity in entities)
                    {
                        try
                        {
                            targetModelSpace.AppendEntity(entity);
                            entitiesAdded++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to add entity for data point at {X}, {Y}", dataPoint.X, dataPoint.Y);
                        }
                    }
                }

                _logger.LogInformation("Successfully imported {EntitiesAdded} entities from SoundPlan data", entitiesAdded);
                return entitiesAdded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during SoundPlan import from {FilePath}", soundPlanFilePath);
                return 0;
            }
        }

        /// <summary>
        /// Parse SoundPlan file into structured data
        /// This method will use your existing DxfToolLib parsing logic
        /// </summary>
        private List<SoundPlanDataPoint> ParseSoundPlanFile(string[] soundPlanLines)
        {
            var dataPoints = new List<SoundPlanDataPoint>();

            try
            {
                // TODO: Use your existing DxfToolLib parser here
                // This is a placeholder implementation - you'll replace this with your actual parsing logic
                
                foreach (var line in soundPlanLines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith(";"))
                        continue;

                    var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        if (double.TryParse(parts[0], out double x) && 
                            double.TryParse(parts[1], out double y))
                        {
                            var dataPoint = new SoundPlanDataPoint
                            {
                                X = x,
                                Y = y,
                                Z = parts.Length > 2 && double.TryParse(parts[2], out double z) ? z : 0,
                                Description = parts.Length > 3 ? parts[3] : "",
                                // Add more properties as needed from your SoundPlan data structure
                            };
                            dataPoints.Add(dataPoint);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing SoundPlan file");
            }

            return dataPoints;
        }

        /// <summary>
        /// Create AutoCAD entities from SoundPlan data point
        /// </summary>
        private List<Entity> CreateAutoCADEntitiesFromSoundPlanData(SoundPlanDataPoint dataPoint, CoordinateTransformOptions? transform)
        {
            var entities = new List<Entity>();

            try
            {
                // Apply coordinate transformation if specified
                var transformedPoint = ApplyCoordinateTransform(new Point3d(dataPoint.X, dataPoint.Y, dataPoint.Z), transform);

                // Create a point entity (or circle for visibility)
                var circle = new Circle
                {
                    Center = transformedPoint,
                    Radius = 0.5, // Small circle to represent the point
                    Color = Color.FromColorIndex(ColorMethod.ByColor, 1) // Red color
                };
                entities.Add(circle);

                // Create text label if description exists
                if (!string.IsNullOrEmpty(dataPoint.Description))
                {
                    var text = new DBText
                    {
                        Position = new Point3d(transformedPoint.X + 1, transformedPoint.Y, transformedPoint.Z),
                        TextString = dataPoint.Description,
                        Height = 1.0,
                        Color = Color.FromColorIndex(ColorMethod.ByColor, 3) // Green color
                    };
                    entities.Add(text);
                }

                // Add more entity types based on your SoundPlan data requirements
                // For example: noise level indicators, measurement points, etc.

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error creating entities for data point at {X}, {Y}", dataPoint.X, dataPoint.Y);
            }

            return entities;
        }

        /// <summary>
        /// Apply coordinate transformation to a point
        /// </summary>
        private Point3d ApplyCoordinateTransform(Point3d originalPoint, CoordinateTransformOptions? transform)
        {
            if (transform == null)
                return originalPoint;

            // Apply scaling
            var scaledPoint = new Point3d(
                originalPoint.X * transform.ScaleFactor,
                originalPoint.Y * transform.ScaleFactor,
                originalPoint.Z * transform.ScaleFactor
            );

            // Apply rotation (around Z-axis)
            if (Math.Abs(transform.RotationAngle) > 1e-10)
            {
                var cos = Math.Cos(transform.RotationAngle);
                var sin = Math.Sin(transform.RotationAngle);
                
                scaledPoint = new Point3d(
                    scaledPoint.X * cos - scaledPoint.Y * sin,
                    scaledPoint.X * sin + scaledPoint.Y * cos,
                    scaledPoint.Z
                );
            }

            // Apply offset
            return new Point3d(
                scaledPoint.X + transform.OffsetX,
                scaledPoint.Y + transform.OffsetY,
                scaledPoint.Z + transform.OffsetZ
            );
        }
#endif
    }

    /// <summary>
    /// Represents a single data point from SoundPlan file
    /// </summary>
    public class SoundPlanDataPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string Description { get; set; } = string.Empty;
        
        // Add more properties as needed for your SoundPlan data
        public string? NoiseLevel { get; set; }
        public string? MeasurementType { get; set; }
        public DateTime? MeasurementDate { get; set; }
        public Dictionary<string, object> AdditionalProperties { get; set; } = new();
    }
}
