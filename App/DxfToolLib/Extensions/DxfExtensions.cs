using System.Text;
using System.Text.RegularExpressions;
using DxfToolLib.Models;
using DxfToolLib.Schemas;

public static class DxfExtensions
{
    public static string[] ToTabSeparatedFormat(this IEnumerable<DxfPointReportItem> dxfPoints)
    {
        return dxfPoints.Select(item =>
            $"{item.Latitude}\t{item.Longitude}\t{item.Height}\t{item.Layer}\t{item.Description}")
            .ToArray();
    }

    // Simple cache for compiled schema regex arrays keyed by schema name + item count
    private static readonly Dictionary<string, Regex[]> _schemaRegexCache = new();

    public static string[] UpdateDxfWithSoundPlanData(this string[] dxfInputLines, DxfPointReportItem[] updatedDxfPoints, PointWithMultiLeaderSchema schema)
    {
        var schemaItems = schema.GetSchemaItems(null);
        if (schemaItems.Count == 0) return (string[])dxfInputLines.Clone();

        // Get or build compiled regex list (anchored, tolerate surrounding whitespace)
        var cacheKey = $"{schema.Name}:{schemaItems.Count}";
        if (!_schemaRegexCache.TryGetValue(cacheKey, out var schemaRegexes))
        {
            schemaRegexes = schemaItems
                .Select(item => new Regex($"^\\s*{item}\\s*$", RegexOptions.Compiled))
                .ToArray();
            _schemaRegexCache[cacheKey] = schemaRegexes;
        }

        var metadata = schema.FieldIndexes;

        int expectedIdx = 0;      // next schema item index
        int sequenceStart = -1;   // starting line index of current candidate (for future replacement logic)
        var pointData = new DxfPoint
        {
            Latitude = "",
            Longitude = "",
            Height = "",
            Layer = "",
            Description = ""
        };

        for (int lineIdx = 0; lineIdx < dxfInputLines.Length; lineIdx++)
        {
            if (expectedIdx == schemaItems.Count)
            {
                expectedIdx = 0;
                sequenceStart = -1;
                pointData = new DxfPoint
                {
                    Latitude = "",
                    Longitude = "",
                    Height = "",
                    Layer = "",
                    Description = ""
                };
            }

            var line = dxfInputLines[lineIdx].Trim();
            var currentRegex = schemaRegexes[expectedIdx];
            var match = currentRegex.Match(line);

            if (match.Success)
            {
                if (sequenceStart == -1) sequenceStart = lineIdx;

                expectedIdx++;
                if (metadata[KnownSchemas.PointWithMultiLeader.FIELDS.LATTITUDE] == expectedIdx)
                {
                    pointData.Latitude = line.Replace(".", ",");
                }
                else if (metadata[KnownSchemas.PointWithMultiLeader.FIELDS.LONGITUDE] == expectedIdx)
                {
                    pointData.Longitude = line.Replace(".", ",");
                }
                else if (metadata[KnownSchemas.PointWithMultiLeader.FIELDS.HEIGHT] == expectedIdx)
                {
                    pointData.Height = line.Replace(".", ",");
                }
                else if (metadata[KnownSchemas.PointWithMultiLeader.FIELDS.LAYER_NAME] == expectedIdx)
                {
                    pointData.Layer = line;
                }
                else if (metadata[KnownSchemas.PointWithMultiLeader.FIELDS.DESCRIPTION] == expectedIdx)
                {
                    var matchPoint = updatedDxfPoints.FirstOrDefault(dp =>
                        dp.Latitude == pointData.Latitude &&
                        dp.Longitude == pointData.Longitude &&
                        dp.Layer == pointData.Layer);
                    var heightLineIdx = lineIdx - expectedIdx + metadata[KnownSchemas.PointWithMultiLeader.FIELDS.HEIGHT];

                    dxfInputLines[lineIdx] = matchPoint?.Description ?? line;
                    dxfInputLines[heightLineIdx] = matchPoint?.Height.Replace(",", ".") ?? dxfInputLines[heightLineIdx];
                }
            }
            else if (expectedIdx > 0)
            {
                // Failed mid-sequence: restart scan at the element after sequenceStart
                lineIdx = sequenceStart;  // for loop increments => resume at sequenceStart+1
                expectedIdx = 0;
                sequenceStart = -1;
                pointData = new DxfPoint
                {
                    Latitude = "",
                    Longitude = "",
                    Height = "",
                    Layer = "",
                    Description = ""
                };
            }
        }

        // NOTE: We no longer store matched capture groups; only detection is performed.
        // Future step: perform direct in-place replacement using sequenceStart & length when a full match occurs.

        return (string[])dxfInputLines.Clone();
    }
}