using DxfToolLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLib.Utils
{
    internal class FindGpsCoordinates
    {
        public static Boolean IsGpsCoordinates(float longitude, float latitude, GpsCoords minCoordinates, GpsCoords maxCoordinates) 
        {
            return (longitude >= minCoordinates.Longitude && latitude >= minCoordinates.Latitude
                && longitude <= maxCoordinates.Longitude && latitude <= maxCoordinates.Latitude);
        }
    }
}
