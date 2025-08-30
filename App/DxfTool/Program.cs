using System;
using System.Windows;
using DxfTool.Views;
using DxfTool.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DxfTool;

class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            // Modern application builder approach
            var builder = Host.CreateApplicationBuilder();
            
            // Register services
            Module.ConfigureServices(builder.Services);
            DxfToolLib.Module.ConfigureServices(builder.Services);
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainWindow>();
            
            var host = builder.Build();
            
            var app = new Application();
            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            
            app.Run(mainWindow);
            
            host.Dispose();
        }
        catch (Exception ex)
        {
            // Print detailed error to console
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
                                        $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            
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
    }
}