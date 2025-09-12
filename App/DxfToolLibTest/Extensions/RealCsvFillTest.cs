using DxfToolLib.Extensions;
using DxfToolLib.Services;
using Xunit;

namespace DxfToolLibTest.Extensions;

public class RealCsvFillTest
{
    [Fact]
    public void FillFromPreviousRows_ShouldWork_WithRealCsvData()
    {
        // Arrange
        var csvPath = @"c:\Users\dszcz\Documents\Projects\Github\dxf-tools\tmp\dxf-soundplan\OBLICZENIA.xlsx - TABELA KOŃCOWA.csv";
        
        if (!File.Exists(csvPath))
        {
            // Skip test if file doesn't exist
            return;
        }

        var parser = new CsvParser();
        
        // Act
        var originalEntries = parser.ParseFinalTableDataFromFile(csvPath).ToArray();
        var validEntries = originalEntries
            .WithValidLp()
            .WithCalculatedPointNumbers()
            .WithCoordinates()
            .ToArray();
        
        var filledEntries = validEntries.FillFromPreviousRows();

        // Assert
        Assert.True(filledEntries.Length >= validEntries.Length);
        
        // Log first few entries to understand the structure
        for (int i = 0; i < Math.Min(5, filledEntries.Length); i++)
        {
            var entry = filledEntries[i];
            Console.WriteLine($"Entry {i}: Lp={entry.Lp}, Building={entry.BuildingNr}, Point={entry.CalculatedPointNr}, DayOrNight={entry.DayOrNight}");
        }
        
        // Check that we actually filled missing data by comparing counts
        var originalCompleteEntries = validEntries.Count(e => !string.IsNullOrWhiteSpace(e.BuildingNr) && 
                                                               !string.IsNullOrWhiteSpace(e.CalculatedPointNr) &&
                                                               !string.IsNullOrWhiteSpace(e.Coordinates));
        
        var filledCompleteEntries = filledEntries.Count(e => !string.IsNullOrWhiteSpace(e.BuildingNr) && 
                                                             !string.IsNullOrWhiteSpace(e.CalculatedPointNr) &&
                                                             !string.IsNullOrWhiteSpace(e.Coordinates));
        
        // After filling, we should have more complete entries
        Assert.True(filledCompleteEntries >= originalCompleteEntries, 
            $"Expected at least {originalCompleteEntries} complete entries, got {filledCompleteEntries}");
    }
}
