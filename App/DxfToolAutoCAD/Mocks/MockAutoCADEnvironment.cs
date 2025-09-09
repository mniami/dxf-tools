using System;
using System.Collections.Generic;

namespace DxfToolAutoCAD.Mocks
{
    /// <summary>
    /// Mock implementation for testing AutoCAD functionality without AutoCAD installed
    /// </summary>
    public class MockAutoCADEnvironment
    {
        /// <summary>
        /// Simulate importing SoundPlan data without AutoCAD
        /// </summary>
        public static void TestSoundPlanImport()
        {
            Console.WriteLine("=== Mock AutoCAD SoundPlan Import Test ===");
            
            // Create sample SoundPlan data
            var sampleData = CreateSampleSoundPlanData();
            
            // Simulate the import process
            Console.WriteLine($"Processing {sampleData.Count} SoundPlan entries...");
            
            foreach (var data in sampleData)
            {
                Console.WriteLine($"Entity {data.Idx}: Point({data.Latitude}, {data.Longitude}, {data.Height})");
                Console.WriteLine($"  Sound Data: Lrd={data.Lrd}, Lrn={data.Lrn}, Lrdn={data.Lrdn}");
                Console.WriteLine($"  Would create: Point, Circle(radius=1.0), Text labels");
                Console.WriteLine($"  Layers: SoundPlan_Points, SoundPlan_Text");
                Console.WriteLine();
            }
            
            Console.WriteLine($"Mock import completed: {sampleData.Count} entities would be created");
        }
        
        /// <summary>
        /// Test coordinate transformation logic
        /// </summary>
        public static void TestCoordinateTransformation()
        {
            Console.WriteLine("=== Coordinate Transformation Test ===");
            
            var transform = new CoordinateTransformOptions
            {
                OffsetX = 100.0,
                OffsetY = 200.0,
                OffsetZ = 10.0,
                ScaleFactor = 1.5,
                RotationAngle = Math.PI / 4 // 45 degrees
            };
            
            var originalPoint = new { X = 50.0, Y = 30.0, Z = 5.0 };
            
            // Apply transformations (simplified logic)
            var x = (originalPoint.X + transform.OffsetX) * transform.ScaleFactor;
            var y = (originalPoint.Y + transform.OffsetY) * transform.ScaleFactor;
            var z = (originalPoint.Z + transform.OffsetZ) * transform.ScaleFactor;
            
            // Apply rotation
            var cos = Math.Cos(transform.RotationAngle);
            var sin = Math.Sin(transform.RotationAngle);
            var rotatedX = x * cos - y * sin;
            var rotatedY = x * sin + y * cos;
            
            Console.WriteLine($"Original point: ({originalPoint.X}, {originalPoint.Y}, {originalPoint.Z})");
            Console.WriteLine($"After offset: ({originalPoint.X + transform.OffsetX}, {originalPoint.Y + transform.OffsetY}, {originalPoint.Z + transform.OffsetZ})");
            Console.WriteLine($"After scale: ({x:F2}, {y:F2}, {z:F2})");
            Console.WriteLine($"After rotation: ({rotatedX:F2}, {rotatedY:F2}, {z:F2})");
        }
        
        /// <summary>
        /// Create sample SoundPlan data for testing
        /// </summary>
        private static List<SoundPlanData> CreateSampleSoundPlanData()
        {
            return new List<SoundPlanData>
            {
                new SoundPlanData { Idx = 1, Latitude = "52.5200", Longitude = "13.4050", Height = "34.0", Lrd = "65.2", Lrn = "58.1", Lrdn = "45.3" },
                new SoundPlanData { Idx = 2, Latitude = "52.5250", Longitude = "13.4100", Height = "36.5", Lrd = "67.8", Lrn = "60.2", Lrdn = "47.1" },
                new SoundPlanData { Idx = 3, Latitude = "52.5300", Longitude = "13.4150", Height = "38.2", Lrd = "63.4", Lrn = "56.8", Lrdn = "43.9" },
                new SoundPlanData { Idx = 4, Latitude = "52.5350", Longitude = "13.4200", Height = "35.8", Lrd = "69.1", Lrn = "62.5", Lrdn = "49.2" },
                new SoundPlanData { Idx = 5, Latitude = "52.5400", Longitude = "13.4250", Height = "37.1", Lrd = "64.7", Lrn = "57.9", Lrdn = "44.6" }
            };
        }
        
        /// <summary>
        /// Test file parsing logic
        /// </summary>
        public static void TestSoundPlanFileParsing()
        {
            Console.WriteLine("=== SoundPlan File Parsing Test ===");
            
            // Sample SoundPlan file content
            var sampleFileContent = new string[]
            {
                "Idx\tLatitude\tLongitude\tHeight\tLrd\tLrn\tLrdn",
                "1\t52.5200\t13.4050\t34.0\t65.2\t58.1\t45.3",
                "2\t52.5250\t13.4100\t36.5\t67.8\t60.2\t47.1",
                "3\t52.5300\t13.4150\t38.2\t63.4\t56.8\t43.9"
            };
            
            Console.WriteLine("Sample file content:");
            foreach (var line in sampleFileContent)
            {
                Console.WriteLine($"  {line}");
            }
            
            Console.WriteLine("\nParsed data:");
            // Skip header
            for (int i = 1; i < sampleFileContent.Length; i++)
            {
                var parts = sampleFileContent[i].Split('\t');
                if (parts.Length >= 7)
                {
                    Console.WriteLine($"  Point {parts[0]}: ({parts[1]}, {parts[2]}, {parts[3]}) - Sound: {parts[4]}/{parts[5]}/{parts[6]}");
                }
            }
        }
    }
}
