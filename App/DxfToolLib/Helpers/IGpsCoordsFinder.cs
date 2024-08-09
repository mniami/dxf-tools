using DxfToolLib.Models;

namespace DxfToolLib.Helpers
{
    internal interface IGpsCoordsFinder
    {
        IEnumerable<GpsCoords> Find(string[] input, GpsCoords minCoordinates, GpsCoords maxCoordinates);
    }
}