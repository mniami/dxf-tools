using DxfToolLib.Extensions;
using DxfToolLib.Models;
using Xunit;
using Xunit.Abstractions;

namespace DxfToolLibTest.Extensions;

public class FillFromPreviousRowsDiagnosticTest
{
    private readonly ITestOutputHelper _output;

    public FillFromPreviousRowsDiagnosticTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void DiagnoseCurrentBehavior()
    {
        // Arrange - Test with entries that should be considered complete based on the algorithm
        var entries = new FinalTableData[]
        {
            // Complete entry (all key fields: BuildingNr, CalculatedPointNr, Coordinates)
            new() 
            { 
                Lp = "1", BuildingNr = "Building1", StageNr = "Stage1", 
                CalculatedPointNr = "Point1", Coordinates = "100;200", RoomType = "room1"
            },
            
            // Incomplete entry (missing key fields)
            new() { Lp = "2", DayOrNight = "test" },
            
            // Entry with ERROR in CalculatedPointNr (should not be complete)
            new() 
            { 
                Lp = "3", BuildingNr = "Building2", StageNr = "Stage2",
                CalculatedPointNr = "#ERROR!", Coordinates = "300;400"
            },
            
            // Another incomplete
            new() { Lp = "4", DayOrNight = "test2" },
            
            // Another complete entry
            new() 
            { 
                Lp = "5", BuildingNr = "Building3", StageNr = "Stage3",
                CalculatedPointNr = "Point3", Coordinates = "500;600", RoomType = "room3"
            },
            
            // Final incomplete
            new() { Lp = "6", DayOrNight = "test3" }
        };

        // Act
        var filled = entries.FillFromPreviousRows();

        // Assert & Debug
        for (int i = 0; i < filled.Length; i++)
        {
            _output.WriteLine($"Entry {i}: Lp={filled[i].Lp}, Building={filled[i].BuildingNr}, Point={filled[i].CalculatedPointNr}, Day={filled[i].DayOrNight}, Coords={filled[i].Coordinates}, Room={filled[i].RoomType}");
        }

        Assert.Equal(6, filled.Length);
        
        // Entry 0 should be unchanged (complete)
        Assert.Equal("Building1", filled[0].BuildingNr);
        Assert.Equal("Point1", filled[0].CalculatedPointNr);
        
        // Entry 1 should inherit from entry 0
        _output.WriteLine($"Entry 1 inheritance check:");
        _output.WriteLine($"  BuildingNr: '{filled[1].BuildingNr}' (should be 'Building1')");
        _output.WriteLine($"  CalculatedPointNr: '{filled[1].CalculatedPointNr}' (should be 'Point1')");
        _output.WriteLine($"  RoomType: '{filled[1].RoomType}' (should be 'room1')");
        _output.WriteLine($"  DayOrNight: '{filled[1].DayOrNight}' (should be 'test')");
        
        // Entry 2 has ERROR in key field, should not be complete
        _output.WriteLine($"Entry 2 ERROR handling:");
        _output.WriteLine($"  CalculatedPointNr: '{filled[2].CalculatedPointNr}' (should show inheritance or ERROR)");
        
        // Entry 3 should inherit from last complete entry
        _output.WriteLine($"Entry 3 inheritance check:");
        _output.WriteLine($"  BuildingNr: '{filled[3].BuildingNr}' (inheritance source?)");
        
        // Entries 4-5 test new complete entry and subsequent inheritance
        Assert.Equal("Building3", filled[4].BuildingNr);
        Assert.Equal("Point3", filled[4].CalculatedPointNr);
        
        _output.WriteLine($"Entry 5 inheritance from new complete:");
        _output.WriteLine($"  BuildingNr: '{filled[5].BuildingNr}' (should be 'Building3')");
        _output.WriteLine($"  CalculatedPointNr: '{filled[5].CalculatedPointNr}' (should be 'Point3')");
    }
}
