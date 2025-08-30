using DxfToolLib.Schemas.Core;
using DxfToolLib.Utils;

namespace DxfToolLib.Schemas
{
    public class GeometryPointAutoCad2004Schema: ISchema
    {
        public string Name
        {
            get
            {
                return KnownSchemas.GeometryPointAutoCad2004.NAME;
            }
        }

        public IList<string> GetSchemaItems(IDictionary<string, string>? variables)
        {
            return [
                "AcDbPoint",
                "10",
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",
                "20",
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",
                "30",
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",
                "0",
            ];
        }
    }
}
