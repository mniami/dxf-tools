using DxfToolLib.Helpers;
using DxfToolLib.Schemas;
using DxfToolLib.Schemas.Core;
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
            services.AddSingleton<ISchema, HighPointAutoCad2004Schema>();
            services.AddSingleton<ISchema, HighPointAutoCad2000Schema>();
            services.AddSingleton<ISchema, PointWithMultiLeaderSchema>();
            services.AddSingleton<ISchema, CodePageSchema>();
            services.AddSingleton<ISchema, CadVersionSchema>();
            services.AddSingleton<ISchema, GpsCoordsSchema>();
            services.AddTransient<IDxfService, DxfService>();
            services.AddTransient<ISchemaFinder, SchemaFinder>();
            services.AddTransient<IMatchesFinder, MatchesFinder>();
            services.AddTransient<IGpsCoordsFinder, GpsCoordsFinder>();
        }
    }
}
