
namespace DxfToolLib.Helpers;
internal static class SoundPlanFileMapperExtensions
{
    public static SoundPlanData[] MapDxfToSoundPlan(this string[] dxfLines)
    {
        return dxfLines.Skip(3).Select(line => line.Split('\t')).Select(line => new SoundPlanData
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
