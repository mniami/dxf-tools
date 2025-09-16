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
    public static DxfPoint[] MapWithReportItems(this DxfPoint[] dxfPoints, ReportItemData[] reportItems, ILogger? logger = null)
    {
        var dxfPointsWithReportMatches = dxfPoints.Select(dxfPoint =>
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

            // Create a copy of the DxfPoint with additional report data if match found
            var result = new DxfPoint
            {
                Latitude = dxfPoint.Latitude.Replace(".", ","),
                Longitude = dxfPoint.Longitude.Replace(".", ","),
                Height = dxfPoint.Height,
                Layer = dxfPoint.Layer,
                Description = dxfPoint.Description
            };

            // If we found a matching report item, enhance description with report data
            if (matchingReportItem != null)
            {
                // Parse heights and calculate sum
                var currentHeight = double.TryParse(result.Height, out var currentHeightValue) ? currentHeightValue : 0;
                var additionalHeight = double.TryParse(matchingReportItem.AdditionalHeight, out var additionalHeightValue) ? additionalHeightValue : 0;
                result.Height = (currentHeight + additionalHeight).ToString();
            }

            var oldLabel = result.Description.Split(';')[2].Split('}')[0];
            var newLabel = matchingReportItem?.CalculatedPointNr ?? oldLabel;
            if (oldLabel != newLabel)
            {
                result.Description = result.Description.Replace(oldLabel, newLabel);
                logger?.LogInformation("Updated label from '{OldLabel}' to '{NewLabel}' for point at ({Latitude}, {Longitude})", oldLabel, newLabel, result.Latitude, result.Longitude);
            }
            return result;
        }).ToArray();

        return dxfPointsWithReportMatches;
    }
}
