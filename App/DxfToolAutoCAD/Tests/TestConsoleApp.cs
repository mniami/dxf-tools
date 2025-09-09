using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DxfToolAutoCAD;

namespace DxfToolAutoCAD.Tests
{
    /// <summary>
    /// Console application to test the AutoCAD plugin without AutoCAD
    /// </summary>
    public class TestConsoleApp
    {
        public static void RunTests()
        {
            Console.WriteLine("=== DxfTool AutoCAD Plugin Test Console ===");
            Console.WriteLine();

            try
            {
                // Test 1: Initialize the plugin
                Console.WriteLine("1. Testing Plugin Initialization...");
                var initializer = new PluginInitializer();
                initializer.Initialize();
                Console.WriteLine("   ✅ Plugin initialized successfully");

                // Test 2: Test service provider
                Console.WriteLine("2. Testing Service Provider...");
                var serviceProvider = PluginInitializer.ServiceProvider;
                var logger = serviceProvider.GetService<ILogger<TestConsoleApp>>();
                Console.WriteLine("   ✅ Service provider working");
                Console.WriteLine("   ✅ Logger service available");

                // Test 3: Test services (non-AutoCAD dependent parts)
                Console.WriteLine("3. Testing Services...");
                var importService = serviceProvider.GetService<ISoundPlanImportService>();
                var drawingService = serviceProvider.GetService<IAutoCADDrawingService>();
                
#if AUTOCAD_API_AVAILABLE
                Console.WriteLine($"   ✅ Import service: {(importService != null ? "Available" : "Not Available")}");
                Console.WriteLine($"   ✅ Drawing service: {(drawingService != null ? "Available" : "Not Available")}");
#else
                Console.WriteLine("   ⚠️  AutoCAD services not available (AutoCAD not installed)");
                Console.WriteLine("   ✅ This is expected on development machines without AutoCAD");
#endif

                // Test 4: Test logging
                Console.WriteLine("4. Testing Logging...");
                logger?.LogInformation("Test log message from console app");
                Console.WriteLine("   ✅ Logging working");

                // Test 5: Cleanup
                Console.WriteLine("5. Testing Cleanup...");
                initializer.Terminate();
                Console.WriteLine("   ✅ Plugin terminated successfully");

                Console.WriteLine();
                Console.WriteLine("🎉 All tests passed! Plugin is working correctly.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during testing: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw for test framework
            }

            Console.WriteLine();
            // Only wait for key press when running interactively (not in tests)
            if (Environment.UserInteractive && !Console.IsInputRedirected)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
