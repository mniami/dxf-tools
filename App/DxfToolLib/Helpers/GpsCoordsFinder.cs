using DxfToolLib.Models;
using DxfToolLib.Schemas;
using DxfToolLib.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLib.Helpers
{
    internal class GpsCoordsFinder : IGpsCoordsFinder
    {
        private readonly ISchemaFinder schemaFinder;

        public GpsCoordsFinder(ISchemaFinder schemaFinder)
        {
            this.schemaFinder = schemaFinder;
        }

        public IEnumerable<GpsCoords> Find(string[] input, GpsCoords minCoordinates, GpsCoords maxCoordinates)
        {
            var matches = schemaFinder.Matches(KnownSchemas.GpsCoords.NAME, null, input);
            if (matches == null)
            {
                yield break;
            }
            foreach (var match in matches.CombineMatches())
            {
                var groups = match.Split(',');
                var longitude = float.Parse(groups[0], CultureInfo.InvariantCulture);
                var latitude = float.Parse(groups[1], CultureInfo.InvariantCulture);
                var height = float.Parse(groups[2], CultureInfo.InvariantCulture);

                if (FindGpsCoordinates.IsGpsCoordinates(longitude, latitude, minCoordinates, maxCoordinates))
                {
                    yield return new GpsCoords
                    {
                        Latitude = latitude,
                        Longitude = longitude,
                        Height = height,
                    };
                }
            }
        }
    }
}
