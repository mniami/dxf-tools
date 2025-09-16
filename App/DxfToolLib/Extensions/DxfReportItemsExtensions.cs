using DxfToolLib.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace DxfToolLib.Extensions;

public static class DxfReportItemsExtensions
{
    private static int TryParseCoordinate(string coordinate)
    {
        return int.TryParse(System.Text.RegularExpressions.Regex.Replace(coordinate.Trim(), "\\s+", ""), out var result) ? result : 0;
    }
    /// <summary>
    /// Maps DXF points with report items based on coordinate matching (X,Y with Latitude,Longitude)
    /// </summary>
    /// <param name="dxfPoints">The DXF points to map</param>
    /// <param name="reportItems">The report items to match with</param>
    /// <param name="logger">Optional logger for tracking matches</param>
    /// <returns>Array of DXF points enhanced with report data where matches are found</returns>
    public static DxfPointReportItem[] MapWithReportItems(this DxfPoint[] dxfPoints, ReportItemData[] reportItems, ILogger? logger = null)
    {
        return dxfPoints.Select(dxfPoint =>
        {
            // Find matching report item by coordinates
            var dxfPointX = (int)Math.Round(Double.Parse(dxfPoint.Latitude.Replace(".", ",")), MidpointRounding.AwayFromZero);
            var dxfPointY = (int)Math.Round(Double.Parse(dxfPoint.Longitude.Replace(".", ",")), MidpointRounding.AwayFromZero);
            var reportItemCoordinates = reportItems.Select(ri => new
            {
                ri,
                x = ri.X == null ? 0 : TryParseCoordinate(ri.X),
                y = ri.Y == null ? 0 : TryParseCoordinate(ri.Y)
            }).ToArray();
            var matchingReportItem = reportItemCoordinates.FirstOrDefault(reportItem =>
                reportItem.x == dxfPointX &&
                reportItem.y == dxfPointY)?.ri;

            var oldLabel = dxfPoint.Description.Split(';')[2].Split('}')[0];
            var newLabel = matchingReportItem?.CalculatedPointNr ?? oldLabel;
            var description = dxfPoint.Description.Replace(oldLabel, newLabel);
            // Create a copy of the DxfPoint with additional report data if match found
            return new DxfPointReportItem
            {
                Latitude = dxfPoint.Latitude.Replace(".", ","),
                Longitude = dxfPoint.Longitude.Replace(".", ","),
                Height = dxfPoint.Height,
                Layer = dxfPoint.Layer,
                Description = description,
                AdditionalHeight = matchingReportItem?.AdditionalHeight ?? String.Empty,
            };
        }).ToArray();
    }
}
