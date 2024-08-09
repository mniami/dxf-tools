using DxfToolLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLib.Helpers
{
    internal class Poland
    {
        public readonly GpsCoords Min = new()
        {
            Latitude = 6523599.54f,
            Longitude = 5931087.52f,
            Height = 0f,
        };
        public readonly GpsCoords Max = new()
        {
            Latitude = 6543599.54f,
            Longitude = 6631087.52f,
            Height = 5000f,
        };
    }
    internal class KnownGpsCoords
    {
        public static readonly Poland Poland = new();
    }
}
