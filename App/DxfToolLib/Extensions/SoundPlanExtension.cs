
using DxfToolLib.Models;

namespace DxfToolLib.Helpers;
internal static class SoundPlanFileMapperExtensions
{
    public static SoundPlanPoint[] ParseSoundPlanPoints(this string[] soundPlanLines)
    {
        return soundPlanLines.Skip(3)
        .Where(line => line.Trim().Length > 0)
        .Select(line => line.Split('\t'))
        .Select(line => new SoundPlanPoint
            {
                Idx = int.Parse(line[0]),
                Latitude = line[1],
                Longitude = line[2],
                Height = line[3],
                Lrd = line[4],
                Lrn = line[5],
                Lrdn = line[6]
            }).ToArray();
    }
}
