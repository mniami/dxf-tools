using DxfToolLib.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace DxfToolLib.Extensions;

public static class DxfReportItemsExtensions
{
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
            var matchingReportItem = reportItems.FirstOrDefault(reportItem =>
                string.Equals(reportItem.X?.Trim(), dxfPoint.Latitude?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(reportItem.Y?.Trim(), dxfPoint.Longitude?.Trim(), StringComparison.OrdinalIgnoreCase));

            // Create a copy of the DxfPoint with additional report data if match found
            var result = new DxfPoint
            {
                Latitude = dxfPoint.Latitude,
                Longitude = dxfPoint.Longitude,
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

            return result;
        }).ToArray();
        
        logger?.LogInformation("Mapped {MatchCount} DXF points with report data out of {TotalDxfPoints} total DXF points", 
            dxfPointsWithReportMatches.Count(p => p.Description?.Contains("[CalcPt:") == true), 
            dxfPoints.Length);
        
        return dxfPointsWithReportMatches;
    }
}
