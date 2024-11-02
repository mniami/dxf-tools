using DxfTool.Actions;
using DxfTool.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfTool
{
    internal class Module
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<FindAllGpsCoordsAction>();
            services.AddSingleton<FindHighPointsAction>();
            services.AddSingleton<MainViewModel>();
        }
    }
}
