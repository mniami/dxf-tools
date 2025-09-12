using DxfToolLib.Extensions;
using DxfToolLib.Models;
using Xunit;

namespace DxfToolLibTest.Extensions;

public class FillFromPreviousRowsTest
{
    [Fact]
    public Task FillFromPreviousRows_ShouldFillMissingData_FromPreviousCompleteRow()
    {
        // Arrange - simulate CSV structure like in your file
        var entries = new FinalTableData[]
        {
            // Complete row (like row 1 in your CSV)
            new() { Lp = "1", BuildingNr = "9", StageNr = "0", CalculatedPointNr = "1-9/0", 
                   AdditionalHeight = "+2,40m", AppartmentNr = "2", RoomType = "pokój nr 4", 
                   DayOrNight = "pora dzienna", Coordinates = "6 500 104;5 885 729" },
            
            // Incomplete row (like row 2 in your CSV - only DayOrNight changes)
            new() { Lp = "", DayOrNight = "pora nocna" },
            
            // Another incomplete row
            new() { Lp = "", DayOrNight = "pora dzienna", RoomType = "salon" },
            
            // New complete row (like starting a new point)
            new() { Lp = "4", BuildingNr = "10", StageNr = "I", CalculatedPointNr = "2-10/I",
                   AdditionalHeight = "+5,45m", AppartmentNr = "5", RoomType = "kuchnia",
                   DayOrNight = "pora dzienna", Coordinates = "6 500 200;5 885 800" },
            
            // Incomplete row inheriting from the new complete row
            new() { Lp = "", DayOrNight = "pora nocna" }
        };

        // Act
        var filled = entries.FillFromPreviousRows();

        // Assert using Verify for snapshot testing
        return Verify(filled);
    }

    [Fact]
    public Task FillFromPreviousRows_ShouldHandleEmptyArray()
    {
        // Arrange
        var entries = Array.Empty<FinalTableData>();

        // Act
        var filled = entries.FillFromPreviousRows();

        // Assert using Verify
        return Verify(filled);
    }

    [Fact]
    public Task FillFromPreviousRows_ShouldHandleSingleEntry()
    {
        // Arrange
        var entries = new[] { new FinalTableData { Lp = "1", BuildingNr = "9" } };

        // Act
        var filled = entries.FillFromPreviousRows();

        // Assert using Verify
        return Verify(filled);
    }
}
