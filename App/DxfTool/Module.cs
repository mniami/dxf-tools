using DxfTool.Services;
using DxfTool.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DxfTool
{
    internal class Module
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUpdateService, UpdateService>();
            services.AddSingleton<MainViewModel>();
        }
    }
}
