using DxfToolLib.Helpers;
using DxfToolLib.Schemas;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLib
{
    public class Module
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ISchemaStorage, SchemaStorage>();
            services.AddTransient<ISchema, HighPointAutoCad2000Schema>();
            services.AddTransient<IDxfParser, DxfParser>();
        }
    }
}
