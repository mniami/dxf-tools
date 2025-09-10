#if AUTOCAD_API_AVAILABLE
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
#endif
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DxfToolLib;

namespace DxfToolAutoCAD
{
    /// <summary>
    /// Main initialization class for the AutoCAD plugin
    /// </summary>
#if AUTOCAD_API_AVAILABLE
    public class PluginInitializer : IExtensionApplication
#else
    public class PluginInitializer
#endif
    {
        private static IServiceProvider? _serviceProvider;

        /// <summary>
        /// Gets the service provider for dependency injection
        /// </summary>
        public static IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("Plugin not initialized");

        /// <summary>
        /// Called when the plugin is loaded
        /// </summary>
        public void Initialize()
        {
            try
            {
                // Setup dependency injection
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // Write startup message
                var logger = _serviceProvider.GetService<ILogger<PluginInitializer>>();
                logger?.LogInformation("DxfTool AutoCAD Plugin loaded successfully");

#if AUTOCAD_API_AVAILABLE
                Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage("\nDxfTool AutoCAD Plugin loaded successfully.\n");
#endif
            }
            catch (Exception ex)
            {
#if AUTOCAD_API_AVAILABLE
                Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage($"\nError loading DxfTool AutoCAD Plugin: {ex.Message}\n");
#else
                Console.WriteLine($"Error loading DxfTool AutoCAD Plugin: {ex.Message}");
#endif
            }
        }

        /// <summary>
        /// Called when the plugin is unloaded
        /// </summary>
        public void Terminate()
        {
            try
            {
                if (_serviceProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }

#if AUTOCAD_API_AVAILABLE
                Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage("\nDxfTool AutoCAD Plugin unloaded.\n");
#endif
            }
            catch (Exception ex)
            {
#if AUTOCAD_API_AVAILABLE
                Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage($"\nError unloading DxfTool AutoCAD Plugin: {ex.Message}\n");
#else
                Console.WriteLine($"Error unloading DxfTool AutoCAD Plugin: {ex.Message}");
#endif
            }
        }

        /// <summary>
        /// Configure dependency injection services
        /// </summary>
        private void ConfigureServices(IServiceCollection services)
        {
            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Add your DxfToolLib services here
            // TODO: Register your existing DxfToolLib services
            // Example: services.AddTransient<IDxfService, DxfService>();
            
            // Add AutoCAD-specific services
#if AUTOCAD_API_AVAILABLE
            services.AddTransient<ISoundPlanImportService, Services.SoundPlanImportService>();
            services.AddTransient<IAutoCADDrawingService, AutoCADDrawingService>();
#endif
        }
    }
}
