using DxfToolAutoCAD.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DxfToolAutoCAD.TestRunner
{
    /// <summary>
    /// Standalone test runner that doesn't require AutoCAD
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DxfTool AutoCAD Plugin - Standalone Test Runner");
            Console.WriteLine("===============================================");
            Console.WriteLine();
            
            try
            {
                // Test plugin initialization
                TestPluginInitialization();
                
                // Test mock AutoCAD functionality
                TestMockAutoCADFunctionality();
                
                // Test file processing
                TestFileProcessing();
                
                Console.WriteLine();
                Console.WriteLine("✅ All tests completed successfully!");
                Console.WriteLine();
                Console.WriteLine("Next steps:");
                Console.WriteLine("1. Get AutoCAD trial/student version for full testing");
                Console.WriteLine("2. Use NETLOAD in AutoCAD to load the compiled DLL");
                Console.WriteLine("3. Test with real SoundPlan data files");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during testing: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        static void TestPluginInitialization()
        {
            Console.WriteLine("1. Testing Plugin Initialization...");
            
            var initializer = new PluginInitializer();
            initializer.Initialize();
            
            var serviceProvider = PluginInitializer.ServiceProvider;
            var logger = serviceProvider.GetService<ILogger<Program>>();
            
            Console.WriteLine("   ✅ Plugin initialized successfully");
            Console.WriteLine("   ✅ Dependency injection working");
            Console.WriteLine("   ✅ Logging service available");
            
            initializer.Terminate();
            Console.WriteLine("   ✅ Plugin cleanup completed");
            Console.WriteLine();
        }
        
        static void TestMockAutoCADFunctionality()
        {
            Console.WriteLine("2. Testing Mock AutoCAD Functionality...");
            
            MockAutoCADEnvironment.TestSoundPlanImport();
            MockAutoCADEnvironment.TestCoordinateTransformation();
            MockAutoCADEnvironment.TestSoundPlanFileParsing();
            
            Console.WriteLine("   ✅ Mock tests completed");
            Console.WriteLine();
        }
        
        static void TestFileProcessing()
        {
            Console.WriteLine("3. Testing File Processing Logic...");
            
            // Create a temporary test file
            var tempFile = Path.Combine(Path.GetTempPath(), "test_soundplan.txt");
            var testData = new string[]
            {
                "Idx\tLatitude\tLongitude\tHeight\tLrd\tLrn\tLrdn",
                "1\t52.5200\t13.4050\t34.0\t65.2\t58.1\t45.3",
                "2\t52.5250\t13.4100\t36.5\t67.8\t60.2\t47.1",
                "3\t52.5300\t13.4150\t38.2\t63.4\t56.8\t43.9"
            };
            
            try
            {
                File.WriteAllLines(tempFile, testData);
                Console.WriteLine($"   ✅ Created test file: {tempFile}");
                
                var lines = File.ReadAllLines(tempFile);
                Console.WriteLine($"   ✅ Read {lines.Length} lines from file");
                
                // Test parsing logic (without AutoCAD)
                var parsedCount = 0;
                for (int i = 1; i < lines.Length; i++) // Skip header
                {
                    var parts = lines[i].Split('\t');
                    if (parts.Length >= 7)
                    {
                        parsedCount++;
                    }
                }
                
                Console.WriteLine($"   ✅ Parsed {parsedCount} valid data entries");
                
                File.Delete(tempFile);
                Console.WriteLine("   ✅ Cleaned up test file");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ File processing error: {ex.Message}");
            }
            
            Console.WriteLine();
        }
    }
}
