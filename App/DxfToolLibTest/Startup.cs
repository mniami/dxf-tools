using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLibTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            DxfToolLib.Module.ConfigureServices(services);
        }
    }
}
