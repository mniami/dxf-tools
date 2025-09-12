using DxfToolLib.Models;
using System.Globalization;
using System.Reflection;

namespace DxfToolLib.Extensions;

public static class FinalTableDataExtensions
{
    /// <summary>
    /// Generic method to fill missing cells by copying data from previous complete rows.
    /// 
    /// Completeness Logic:
    /// - A row is considered complete if the first string property (index column) has a non-empty value
    /// - Incomplete rows inherit missing values from the last complete row
    /// - Original non-empty values in incomplete rows are preserved
    /// 
    /// Example:
    /// Row 1: Index="1", A="Alpha", B="Beta"    (Complete)
    /// Row 2: Index="",  A="",     B="Modified" (Incomplete, inherits A from Row 1)
    /// Result: Index="", A="Alpha", B="Modified"
    /// </summary>
    public static T[] FillFromPreviousRows<T>(this T[] entries) where T : class, new()
    {
        if (entries?.Length <= 1) return entries ?? Array.Empty<T>();

        var result = new List<T>();
        T? lastComplete = null;
        var properties = typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string)).ToArray();
        
        // First property is considered the index column
        var indexProperty = properties.FirstOrDefault();
        if (indexProperty == null) return entries ?? Array.Empty<T>();

        foreach (var entry in entries!)
        {
            var filled = new T();
            
            // Check if this row is complete (has index value)
            var indexValue = indexProperty.GetValue(entry) as string;
            var isComplete = !string.IsNullOrWhiteSpace(indexValue);

            // Copy all properties with inheritance logic
            foreach (var prop in properties)
            {
                var value = prop.GetValue(entry) as string;
                var isEmpty = string.IsNullOrWhiteSpace(value);
                
                // Inherit from last complete row if current value is empty and we have a source
                prop.SetValue(filled, isEmpty && lastComplete != null ? prop.GetValue(lastComplete) : value);
            }

            // Update lastComplete only if current row is complete
            if (isComplete) lastComplete = filled;
            result.Add(filled);
        }

        return result.ToArray();
    }
    /// <summary>
    /// Filters FinalTableData entries that have valid coordinates
    /// </summary>
    public static IEnumerable<FinalTableData> WithCoordinates(this IEnumerable<FinalTableData> data)
    {
        return data.Where(item => !string.IsNullOrWhiteSpace(item.Coordinates) && 
                                 item.Coordinates != "#N/A" && 
                                 item.Coordinates != "#ERROR!");
    }

    /// <summary>
    /// Filters FinalTableData entries that have valid calculated point numbers
    /// </summary>
    public static IEnumerable<FinalTableData> WithCalculatedPointNumbers(this IEnumerable<FinalTableData> data)
    {
        return data.Where(item => !string.IsNullOrWhiteSpace(item.CalculatedPointNr) && 
                                 item.CalculatedPointNr != "#N/A" && 
                                 item.CalculatedPointNr != "#ERROR!");
    }

    /// <summary>
    /// Groups FinalTableData by building number and stage number
    /// </summary>
    public static IEnumerable<IGrouping<string, FinalTableData>> GroupByBuildingAndStage(this IEnumerable<FinalTableData> data)
    {
        return data.GroupBy(item => $"{item.BuildingNr}-{item.StageNr}");
    }

    /// <summary>
    /// Parses coordinates from the Coordinates field (expected format: "X;Y" or "X,Y")
    /// </summary>
    public static (double? X, double? Y) ParseCoordinates(this FinalTableData data)
    {
        if (string.IsNullOrWhiteSpace(data.Coordinates) || 
            data.Coordinates == "#N/A" || 
            data.Coordinates == "#ERROR!")
        {
            return (null, null);
        }

        try
        {
            var coords = data.Coordinates.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (coords.Length >= 2)
            {
                var xStr = coords[0].Trim().Replace(" ", "");
                var yStr = coords[1].Trim().Replace(" ", "");
                
                // Use invariant culture to parse decimal numbers consistently
                if (double.TryParse(xStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double x) && 
                    double.TryParse(yStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                {
                    return (x, y);
                }
            }
        }
        catch
        {
            // Return null coordinates if parsing fails
        }

        return (null, null);
    }

    /// <summary>
    /// Filters entries that have numeric Lp (sequential number) values
    /// </summary>
    public static IEnumerable<FinalTableData> WithValidLp(this IEnumerable<FinalTableData> data)
    {
        return data.Where(item => !string.IsNullOrWhiteSpace(item.Lp) && 
                                 int.TryParse(item.Lp, out _));
    }

    /// <summary>
    /// Filters entries for day or night time calculations
    /// </summary>
    public static IEnumerable<FinalTableData> ForTimeOfDay(this IEnumerable<FinalTableData> data, bool isDayTime)
    {
        var timeKeyword = isDayTime ? "dzienna" : "nocna";
        return data.Where(item => item.DayOrNight?.Contains(timeKeyword, StringComparison.OrdinalIgnoreCase) == true);
    }
}
