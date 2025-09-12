using System.Text;
using DxfToolLib.Models;

public static class DxfExtensions
{
    public static string[] ToTabSeparatedFormat(this IEnumerable<DxfPoint> dxfPoints)
    {
        return dxfPoints.Select(item => 
            $"{item.Latitude}\t{item.Longitude}\t{item.Height}\t{item.Layer}\t{item.Description}")
            .ToArray();
    }
}   