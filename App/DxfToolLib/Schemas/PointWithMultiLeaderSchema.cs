using DxfToolLib.Schemas.Core;
using DxfToolLib.Utils;

namespace DxfToolLib.Schemas
{
    public class PointWithMultiLeaderSchema : ISchema
    {
        public string Name
        {
            get
            {
                return KnownSchemas.PointWithMultiLeader.NAME;
            }
        }

        public IList<string> GetSchemaItems(IDictionary<string, string>? variables)
        {
            return [
                // Match AcDbPoint section
                "AcDbPoint",
                "10",
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",  // Point X coordinate
                "20", 
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",  // Point Y coordinate
                // Skip lines until we find MULTILEADER
                "30", // 30
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",  // Point Z coordinate
                "0", // 0
                "MULTILEADER",
                "5",
                // Skip lines until we find CONTEXT_DATA
                ".*",
                ".*",
                ".*",
                ".*",
                "AcDbEntity",
                "8",
                "(.*)", // layer name
                "92",
                ".*",
                ..Enumerable.Range(0, 16).SelectMany(_ => new[] { "310", ".*" }),
                "100",
                "AcDbMLeader",
                "300",
                "CONTEXT_DATA\\{",
                "40",
                ".*",
                "10",
                ..Enumerable.Range(0, 22).Select(_ => ".*"),
                $"(.*)",  // Text content containing the note
            ];
        }
    }
}
