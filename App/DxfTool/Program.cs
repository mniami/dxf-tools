using System;
using DxfToolLib.Schemas;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DxfTool;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var host = CreateHostBuilder().Build();

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var services = host.Services;
        var mainForm = services.GetRequiredService<MainForm>();

        Application.Run(mainForm);
    }
    static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
                   .ConfigureServices((context, services) =>
                   {
                       Module.ConfigureServices(services);
                       DxfToolLib.Module.ConfigureServices(services);
                       services.AddSingleton<MainForm>();
                   });
    }
}