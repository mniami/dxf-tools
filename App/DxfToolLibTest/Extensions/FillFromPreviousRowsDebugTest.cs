using DxfToolLib.Extensions;
using DxfToolLib.Models;
using Xunit;
using Xunit.Abstractions;

namespace DxfToolLibTest.Extensions;

public class FillFromPreviousRowsDebugTest
{
    private readonly ITestOutputHelper _output;

    public FillFromPreviousRowsDebugTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestActualBehaviorStep()
    {
        // Simple test case from the working original test
        var entries = new FinalTableData[]
        {
            // Complete entry
            new() 
            { 
                Lp = "1", BuildingNr = "9", StageNr = "0", CalculatedPointNr = "1-9/0", 
                AdditionalHeight = "+2,40m", AppartmentNr = "2", RoomType = "pokój nr 4", 
                DayOrNight = "pora dzienna", Coordinates = "6 500 104;5 885 729" 
            },
            
            // Incomplete entry
            new() { Lp = "2", DayOrNight = "pora nocna" }
        };

        // Act
        var filled = entries.FillFromPreviousRows();

        // Debug output
        _output.WriteLine("=== Results ===");
        for (int i = 0; i < filled.Length; i++)
        {
            _output.WriteLine($"Entry {i}:");
            _output.WriteLine($"  Lp: '{filled[i].Lp}'");
            _output.WriteLine($"  BuildingNr: '{filled[i].BuildingNr}'");
            _output.WriteLine($"  CalculatedPointNr: '{filled[i].CalculatedPointNr}'");
            _output.WriteLine($"  DayOrNight: '{filled[i].DayOrNight}'");
            _output.WriteLine($"  Coordinates: '{filled[i].Coordinates}'");
            _output.WriteLine($"  RoomType: '{filled[i].RoomType}'");
            _output.WriteLine("");
        }

        // Test the actual behavior from the working test
        Assert.Equal("9", filled[1].BuildingNr); // This should work according to original test
        Assert.Equal("1-9/0", filled[1].CalculatedPointNr);
        Assert.Equal("pora nocna", filled[1].DayOrNight);
    }
}
