using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DxfToolLib.Models;

namespace DxfToolLib.Helpers;

internal static partial class DxfSoundPlanMapper
{
    private static string ReplaceSoundPlanPoints(this string description, string lrd, string lrdn)
    {
        var newDescription = description.Insert(description.IndexOf("x;=") + 3, lrd);
        newDescription = newDescription.Insert(newDescription.IndexOf("x;=", newDescription.IndexOf("x;=") + 3) + 3, lrdn);
        return newDescription;
    }
    private static int ParseLayerZIndex(string layer)
    {
        Regex regex = MyRegex();
        var match = regex.Match(layer);
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
    /// <summary>
    /// Update DXF points with SoundPlan data and produces combined SoundPlanPoint objects.
    /// </summary>
    /// <param name="dxfPoints">Collection of DXF points with coordinates and descriptions</param>
    /// <param name="soundPlanPoint">Array of SoundPlan data with additional properties</param>
    /// <returns>Array of matched DxfSoundPlanData objects</returns>
    public static DxfPoint[] UpdateWithSoundPlanData(this IEnumerable<DxfPoint> dxfPoints, SoundPlanPoint[] soundPlanPoints)
    {
        var layers = dxfPoints
            .Select(dp => dp.Layer)
            .Distinct()
            .Select(layer => new { layer, idx = ParseLayerZIndex(layer) })
            .OrderBy(x => x.idx)
            .ToList();
        var matchedData = new List<DxfPoint>();
        var dxfInSoundPlanFormatPoints = dxfPoints.Select(dp => new
        {
            Latitude = dp.Latitude.ToSoundPlanGpsCoordinateFormat(),
            Longitude = dp.Longitude.ToSoundPlanGpsCoordinateFormat(),
            OriginalLatitude = dp.Latitude,
            OriginalLongitude = dp.Longitude,
            dp.Height,
            dp.Description,
            dp.Layer
        }).ToList();

        foreach (var dxfPoint in dxfInSoundPlanFormatPoints)
        {
            var dxfPointLayerIdx = layers.First(l => l.layer == dxfPoint.Layer).idx;
            var matchingSoundPlanPoints = soundPlanPoints.Where(sp =>
                sp.Latitude == dxfPoint.Latitude && sp.Longitude == dxfPoint.Longitude)
                .OrderBy(sp => sp.Height);
            var matchingSoundPlanPoint = matchingSoundPlanPoints
                .Skip(dxfPointLayerIdx)
                .FirstOrDefault();
            var description = matchingSoundPlanPoint != null ? dxfPoint.Description.ReplaceSoundPlanPoints(matchingSoundPlanPoint.Lrd, matchingSoundPlanPoint.Lrdn) : dxfPoint.Description;
            var height = matchingSoundPlanPoint != null ? matchingSoundPlanPoint.Height : dxfPoint.Height;
            var combinedData = new DxfPoint
            {
                Latitude = dxfPoint.OriginalLatitude,
                Longitude = dxfPoint.OriginalLongitude,
                Height = height,
                Description = description,
                Layer = dxfPoint.Layer,
            };
            matchedData.Add(combinedData);
        }

        return matchedData.ToArray();
    }

    [GeneratedRegex(@"(\d+)")]
    private static partial Regex MyRegex();
}
