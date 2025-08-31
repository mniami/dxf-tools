using System;
using System.IO;
using System.Windows;
using DxfTool.Views;
using DxfTool.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace DxfTool;

class Program
{
    [STAThread]
    static void Main()
    {
        // Setup Serilog early for startup logging
        var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DxfTool");
        Directory.CreateDirectory(appDataFolder);
        var logFilePath = Path.Combine(appDataFolder, "DxfTool.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(logFilePath, 
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Log.Information("=== DxfTool Application Starting ===");
            Log.Information("Application version: {Version}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            Log.Information("Log file location: {LogFile}", logFilePath);

            // Modern application builder approach
            var builder = Host.CreateApplicationBuilder();
            
            // Configure Serilog as the logging provider
            builder.Services.AddSerilog();
            
            // Register services
            Module.ConfigureServices(builder.Services);
            DxfToolLib.Module.ConfigureServices(builder.Services);
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainWindow>();
            
            var host = builder.Build();
            
            Log.Information("Services configured successfully");
            
            var app = new Application();
            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            
            Log.Information("Starting WPF application");
            app.Run(mainWindow);
            
            Log.Information("Application shutdown completed");
            host.Dispose();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed");
            
            // Print detailed error to console for debugging
            Console.WriteLine("=== APPLICATION STARTUP ERROR ===");
            Console.WriteLine($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Error Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack Trace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("================================");
            
            // Create detailed error message for dialog
            string detailedErrorMessage = $"Error Type: {ex.GetType().Name}\n\n" +
                                        $"Message: {ex.Message}\n\n" +
                                        $"Stack Trace:\n{ex.StackTrace}\n\n" +
                                        $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n" +
                                        $"Log file: {logFilePath}";
            
            // Show custom error dialog that allows copying
            try
            {
                ErrorDialog.ShowError(detailedErrorMessage, "Application Startup Error");
            }
            catch
            {
                // Fallback to MessageBox if ErrorDialog fails
                MessageBox.Show($"Application startup error: {ex.Message}. {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}