using netDxf.Header;
using netDxf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using DxfToolLib.Schemas;
using DxfToolLib.Schemas.Core;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.ComponentModel;

namespace DxfToolLib.Helpers;

internal static class SoundPlanGpsMapperExtension {
    public static string ToSoundPlanGpsCoordinateFormat(this string gpsCoordinate) {
        var x = Math.Round(double.Parse(gpsCoordinate, CultureInfo.InvariantCulture), 3, MidpointRounding.AwayFromZero);
        // Use a culture with comma as decimal separator or replace manually
        return x.ToString("F3", CultureInfo.InvariantCulture).Replace('.', ',');
    }
}