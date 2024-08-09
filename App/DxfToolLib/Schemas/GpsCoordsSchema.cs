using DxfToolLib.Schemas.Core;
using DxfToolLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLib.Schemas
{
    internal class GpsCoordsSchema: ISchema
    {
        public string Name
        {
            get
            {
                return KnownSchemas.GpsCoords.NAME;
            }
        }

        public IList<string> GetSchemaItems(IDictionary<string, string>? variables)
        {
            return [
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",
                ".*",
                ".*",
                ".*",
                ".*",
                ".*",
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})"
            ];
        }
    }
}
