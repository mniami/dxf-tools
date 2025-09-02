using System.Collections.Generic;
using System.Linq;

namespace DxfToolLib.Helpers;

internal static class DxfSoundPlanMapper
{
    /// <summary>
    /// Matches DXF points with SoundPlan data and produces combined DxfSoundPlanData objects.
    /// </summary>
    /// <param name="dxfPoints">Collection of DXF points with coordinates and descriptions</param>
    /// <param name="soundPlanData">Array of SoundPlan data with additional properties</param>
    /// <returns>Array of matched DxfSoundPlanData objects</returns>
    public static DxfSoundPlanData[] MatchDxfWithSoundPlan(IEnumerable<DxfPoint> dxfPoints, SoundPlanData[] soundPlanData)
    {
        var matchedData = new List<DxfSoundPlanData>();
        
        foreach (var dxfPoint in dxfPoints)
        {
            // Find matching SoundPlan data based on coordinates
            var matchingSoundPlanPoint = soundPlanData.FirstOrDefault(sp => 
                sp.Latitude == dxfPoint.Latitude && sp.Longitude == dxfPoint.Longitude);
            
            if (matchingSoundPlanPoint != null)
            {
                var combinedData = new DxfSoundPlanData
                {
                    Idx = matchingSoundPlanPoint.Idx,
                    Latitude = dxfPoint.Latitude,
                    Longitude = dxfPoint.Longitude,
                    Height = matchingSoundPlanPoint.Height,
                    Lrd = matchingSoundPlanPoint.Lrd,
                    Lrn = matchingSoundPlanPoint.Lrn,
                    Lrdn = matchingSoundPlanPoint.Lrdn,
                    Description = dxfPoint.Description
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
    public static string[] ToTabSeparatedFormat(DxfSoundPlanData[] data)
    {
        return data.Select(item => 
            $"{item.Idx}\t{item.Latitude}\t{item.Longitude}\t{item.Height}\t{item.Lrd}\t{item.Lrn}\t{item.Lrdn}\t{item.Description}")
            .ToArray();
    }

    /// <summary>
    /// Matches DXF points with SoundPlan data and returns the result in tab-separated string format.
    /// </summary>
    /// <param name="dxfPoints">Collection of DXF points with coordinates and descriptions</param>
    /// <param name="soundPlanData">Array of SoundPlan data with additional properties</param>
    /// <returns>Array of tab-separated strings</returns>
    public static string[] MatchAndFormat(IEnumerable<DxfPoint> dxfPoints, SoundPlanData[] soundPlanData)
    {
        var matchedData = MatchDxfWithSoundPlan(dxfPoints, soundPlanData);
        return ToTabSeparatedFormat(matchedData);
    }
}
