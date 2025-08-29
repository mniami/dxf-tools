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
            MessageBox.Show($"Application startup error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}