using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DxfToolLib.Models;

namespace DxfToolLib.Helpers;

internal static partial class DxfSoundPlanMapper
{
    private static string replaceSoundPlanPoints(this string description, string lrd, string lrdn)
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
    /// Matches DXF points with SoundPlan data and produces combined SoundPlanPoint objects.
    /// </summary>
    /// <param name="dxfPoints">Collection of DXF points with coordinates and descriptions</param>
    /// <param name="soundPlanPoint">Array of SoundPlan data with additional properties</param>
    /// <returns>Array of matched DxfSoundPlanData objects</returns>
    public static DxfPoint[] MatchDxfWithSoundPlan(IEnumerable<DxfPoint> dxfPoints, SoundPlanPoint[] soundPlanPoints)
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
            if (matchingSoundPlanPoint != null)
            {
                var description = dxfPoint.Description.replaceSoundPlanPoints(matchingSoundPlanPoint.Lrd, matchingSoundPlanPoint.Lrdn);
                var combinedData = new DxfPoint
                {
                    Latitude = dxfPoint.OriginalLatitude,
                    Longitude = dxfPoint.OriginalLongitude,
                    Height = matchingSoundPlanPoint.Height,
                    Description = description,
                };
                matchedData.Add(combinedData);
            }
        }

        return matchedData.ToArray();
    }

    /// <summary>
    /// Converts DxfSoundPlanData objects to tab-separated string format for output.
    /// </summary>
    /// <param name="data">Array of DxfSoundPlanData objects</param>
    /// <returns>Array of tab-separated strings</returns>
    public static string[] ToTabSeparatedFormat(DxfPoint[] data)
    {
        return data.Select(item => 
            $"\t{item.Latitude}\t{item.Longitude}\t{item.Height}\t{item.Description}\t{item.Layer}")
            .ToArray();
    }

    /// <summary>
    /// Matches DXF points with SoundPlan data and returns the result in tab-separated string format.
    /// </summary>
    /// <param name="dxfPoints">Collection of DXF points with coordinates and descriptions</param>
    /// <param name="soundPlanPoint">Array of SoundPlan data with additional properties</param>
    /// <returns>Array of tab-separated strings</returns>
    public static string[] MatchAndFormat(IEnumerable<DxfPoint> dxfPoints, SoundPlanPoint[] soundPlanPoint)
    {
        var matchedData = MatchDxfWithSoundPlan(dxfPoints, soundPlanPoint);
        return ToTabSeparatedFormat(matchedData);
    }

    [GeneratedRegex(@"(\d+)")]
    private static partial Regex MyRegex();
}
