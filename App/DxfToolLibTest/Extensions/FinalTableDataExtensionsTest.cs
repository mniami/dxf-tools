using DxfToolLib.Extensions;
using DxfToolLib.Models;
using Xunit;

namespace DxfToolLibTest.Extensions;

public class FinalTableDataExtensionsTest
{
    private static FinalTableData CreateTestData(
        string lp = "1",
        string buildingNr = "Building1",
        string stageNr = "0",
        string calculatedPointNr = "Point1",
        string dayOrNight = "pora dzienna",
        string coordinates = "1000.5;2000.7")
    {
        return new FinalTableData
        {
            Lp = lp,
            BuildingNr = buildingNr,
            StageNr = stageNr,
            CalculatedPointNr = calculatedPointNr,
            DayOrNight = dayOrNight,
            Coordinates = coordinates
        };
    }

    [Fact]
    public void WithCoordinates_FiltersByValidCoordinates()
    {
        // Arrange
        var data = new[]
        {
            CreateTestData(coordinates: "1000.5;2000.7"),
            CreateTestData(lp: "2", coordinates: ""),
            CreateTestData(lp: "3", coordinates: "#N/A"),
            CreateTestData(lp: "4", coordinates: "#ERROR!"),
            CreateTestData(lp: "5", coordinates: "3000.1;4000.2"),
            CreateTestData(lp: "6", coordinates: "   ") // whitespace
        };

        // Act
        var result = data.WithCoordinates().ToArray();

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal("1", result[0].Lp);
        Assert.Equal("5", result[1].Lp);
    }

    [Fact]
    public void WithCalculatedPointNumbers_FiltersByValidPointNumbers()
    {
        // Arrange
        var data = new[]
        {
            CreateTestData(calculatedPointNr: "Point1"),
            CreateTestData(lp: "2", calculatedPointNr: ""),
            CreateTestData(lp: "3", calculatedPointNr: "#N/A"),
            CreateTestData(lp: "4", calculatedPointNr: "#ERROR!"),
            CreateTestData(lp: "5", calculatedPointNr: "Point2"),
            CreateTestData(lp: "6", calculatedPointNr: "   ") // whitespace
        };

        // Act
        var result = data.WithCalculatedPointNumbers().ToArray();

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal("1", result[0].Lp);
        Assert.Equal("5", result[1].Lp);
    }

    [Fact]
    public void WithValidLp_FiltersByNumericLp()
    {
        // Arrange
        var data = new[]
        {
            CreateTestData(lp: "1"),
            CreateTestData(lp: "2"),
            CreateTestData(lp: "abc"),
            CreateTestData(lp: ""),
            CreateTestData(lp: "3"),
            CreateTestData(lp: "   ") // whitespace
        };

        // Act
        var result = data.WithValidLp().ToArray();

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal("1", result[0].Lp);
        Assert.Equal("2", result[1].Lp);
        Assert.Equal("3", result[2].Lp);
    }

    [Fact]
    public void GroupByBuildingAndStage_GroupsCorrectly()
    {
        // Arrange
        var data = new[]
        {
            CreateTestData(lp: "1", buildingNr: "A", stageNr: "0"),
            CreateTestData(lp: "2", buildingNr: "A", stageNr: "0"),
            CreateTestData(lp: "3", buildingNr: "A", stageNr: "1"),
            CreateTestData(lp: "4", buildingNr: "B", stageNr: "0"),
            CreateTestData(lp: "5", buildingNr: "B", stageNr: "1")
        };

        // Act
        var result = data.GroupByBuildingAndStage().ToArray();

        // Assert
        Assert.Equal(4, result.Length); // 4 unique building-stage combinations
        
        var groupA0 = result.First(g => g.Key == "A-0");
        Assert.Equal(2, groupA0.Count());
        
        var groupA1 = result.First(g => g.Key == "A-1");
        Assert.Single(groupA1);
        
        var groupB0 = result.First(g => g.Key == "B-0");
        Assert.Single(groupB0);
        
        var groupB1 = result.First(g => g.Key == "B-1");
        Assert.Single(groupB1);
    }

    [Theory]
    [InlineData("1000.5;2000.7", 1000.5, 2000.7)]
    [InlineData("1000;2000", 1000.0, 2000.0)]
    [InlineData("6500104;5885729", 6500104.0, 5885729.0)]
    [InlineData("6 500 104 ; 5 885 729", 6500104.0, 5885729.0)] // with spaces
    public void ParseCoordinates_WithValidCoordinates_ReturnsCorrectValues(string coordinates, double expectedX, double expectedY)
    {
        // Arrange
        var data = CreateTestData(coordinates: coordinates);

        // Act
        var (x, y) = data.ParseCoordinates();

        // Assert
        Assert.NotNull(x);
        Assert.NotNull(y);
        Assert.Equal(expectedX, x.Value, precision: 6);
        Assert.Equal(expectedY, y.Value, precision: 6);
    }

    [Theory]
    [InlineData("")]
    [InlineData("#N/A")]
    [InlineData("#ERROR!")]
    [InlineData("   ")]
    [InlineData("invalid;format")]
    [InlineData("1000.5")]  // only one coordinate
    [InlineData("not;numbers")]
    public void ParseCoordinates_WithInvalidCoordinates_ReturnsNull(string coordinates)
    {
        // Arrange
        var data = CreateTestData(coordinates: coordinates);

        // Act
        var (x, y) = data.ParseCoordinates();

        // Assert
        Assert.Null(x);
        Assert.Null(y);
    }

    [Theory]
    [InlineData("pora dzienna", true, true)]
    [InlineData("pora nocna", false, true)]
    [InlineData("pora dzienna", false, false)]
    [InlineData("pora nocna", true, false)]
    [InlineData("PORA DZIENNA", true, true)] // case insensitive
    [InlineData("PORA NOCNA", false, true)] // case insensitive
    [InlineData("something else", true, false)]
    [InlineData("something else", false, false)]
    [InlineData("", true, false)]
    public void ForTimeOfDay_FiltersCorrectly(string dayOrNight, bool isDayTime, bool shouldMatch)
    {
        // Arrange
        var data = new[]
        {
            CreateTestData(dayOrNight: dayOrNight)
        };

        // Act
        var result = data.ForTimeOfDay(isDayTime).ToArray();

        // Assert
        if (shouldMatch)
        {
            Assert.Single(result);
        }
        else
        {
            Assert.Empty(result);
        }
    }

    [Fact]
    public void ChainedFilters_WorkTogether()
    {
        // Arrange
        var data = new[]
        {
            CreateTestData(lp: "1", calculatedPointNr: "Point1", coordinates: "1000;2000", dayOrNight: "pora dzienna"),
            CreateTestData(lp: "2", calculatedPointNr: "", coordinates: "1000;2000", dayOrNight: "pora dzienna"), // no point nr
            CreateTestData(lp: "abc", calculatedPointNr: "Point3", coordinates: "1000;2000", dayOrNight: "pora dzienna"), // invalid lp
            CreateTestData(lp: "4", calculatedPointNr: "Point4", coordinates: "", dayOrNight: "pora dzienna"), // no coordinates
            CreateTestData(lp: "5", calculatedPointNr: "Point5", coordinates: "1000;2000", dayOrNight: "pora nocna"), // night time
            CreateTestData(lp: "6", calculatedPointNr: "Point6", coordinates: "3000;4000", dayOrNight: "pora dzienna") // valid
        };

        // Act
        var result = data
            .WithValidLp()
            .WithCalculatedPointNumbers()
            .WithCoordinates()
            .ForTimeOfDay(true) // day time
            .ToArray();

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal("1", result[0].Lp);
        Assert.Equal("6", result[1].Lp);
    }
}
