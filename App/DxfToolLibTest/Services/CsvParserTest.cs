using DxfToolLib.Models;
using DxfToolLib.Services;
using Xunit;

namespace DxfToolLibTest.Services;

public class CsvParserTest
{
    private readonly ICsvParser parser;

    public CsvParserTest()
    {
        parser = new CsvParser();
    }

    [Fact]
    public void ParseFinalTableData_WithNullInput_ReturnsEmpty()
    {
        // Act
        var result = parser.ParseFinalTableData(null!).ToArray();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ParseFinalTableData_WithEmptyInput_ReturnsEmpty()
    {
        // Act
        var result = parser.ParseFinalTableData(Array.Empty<string>()).ToArray();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ParseFinalTableData_WithBasicCsvData_ParsesCorrectly()
    {
        // Arrange - Simple CSV with headers matching our mapping
        var csvLines = new[]
        {
            "L.p.,Nr budynku,Numer kondygnacji,Numer punktu obliczeniowego,Wysokość Z n.p.t. [m],Nr lokalu,Rodzaj pomieszczenia - funkcja,Obliczony poziom dźwięku (dB),Column8,Miarodajny poziom dźwięku A hałasu zewnętrznego w dB,Column10,Opis przegrody,Column12,Column13,Column14,Column15,UWAGI,Column17,Column18",
            "1,Building1,0,Point1,2.5m,Apt1,Room1,Day,NoiseVal,Range1,Index1,Partition1,Acoustic1,Value1,Sound1,Value2,Comment1,Sep1,1000;2000"
        };

        // Act
        var result = parser.ParseFinalTableData(csvLines).ToArray();

        // Assert
        Assert.Single(result);
        var data = result[0];
        Assert.Equal("1", data.Lp);
        Assert.Equal("Building1", data.BuildingNr);
        Assert.Equal("0", data.StageNr);
        Assert.Equal("Point1", data.CalculatedPointNr);
        Assert.Equal("2.5m", data.AdditionalHeight);
        Assert.Equal("Apt1", data.AppartmentNr);
        Assert.Equal("Room1", data.RoomType);
        Assert.Equal("Day", data.DayOrNight);
        Assert.Equal("Comment1", data.Comments);
        Assert.Equal("1000;2000", data.Coordinates);
    }

    [Fact]
    public void ParseFinalTableData_WithQuotedFields_ParsesCorrectly()
    {
        // Arrange - CSV with quoted fields containing commas and quotes
        var csvLines = new[]
        {
            "L.p.,Nr budynku,Numer kondygnacji,Numer punktu obliczeniowego,Wysokość Z n.p.t. [m],Nr lokalu,Rodzaj pomieszczenia - funkcja,Obliczony poziom dźwięku (dB),Column8,Miarodajny poziom dźwięku A hałasu zewnętrznego w dB,Column10,Opis przegrody,Column12,Column13,Column14,Column15,UWAGI,Column17,Column18",
            "\"1\",\"Building, with commas\",\"0\",\"Point1\",\"2,5m\",\"Apt1\",\"Room \"\"quoted\"\" name\",\"Day\",\"NoiseVal\",\"Range1\",\"Index1\",\"Partition1\",\"Acoustic1\",\"Value1\",\"Sound1\",\"Value2\",\"Comment1\",\"Sep1\",\"1000;2000\""
        };

        // Act
        var result = parser.ParseFinalTableData(csvLines).ToArray();

        // Assert
        Assert.Single(result);
        var data = result[0];
        Assert.Equal("1", data.Lp);
        Assert.Equal("Building, with commas", data.BuildingNr);
        Assert.Equal("Room \"quoted\" name", data.RoomType);
        Assert.Equal("2,5m", data.AdditionalHeight);
    }

    [Fact]
    public void ParseFinalTableData_WithRealWorldHeaders_ParsesCorrectly()
    {
        // Arrange - Based on the actual CSV structure seen in the project
        var csvLines = new[]
        {
            "L.p.,Nr budynku,Numer kondygnacji,Numer punktu obliczeniowego,Wysokość Z n.p.t. [m],Nr lokalu,Rodzaj pomieszczenia - funkcja,Obliczony poziom dźwięku (dB),,Miarodajny poziom dźwięku A hałasu zewnętrznego w dB ,,Opis przegrody,Wymagany wskaźnik oceny izolacyjności akustycznej poszczególnych częsci przegród zewnetrznych właściwej elementu przegrody RA2(dB),,Wymagany wskaźnik izolacyjności akustycznej właściwej elementu przegrody Rw (dB),,UWAGI,,,,,",
            "1,9,0,1-9/0,\"+2,40m\",2,pokój nr 4,pora dzienna,#N/A,do 45,20,system ścian strukturalnych Ponzio NT152ESG,część pełna,25,część pełna,30,,,6 500 104;5 885 729,,"
        };

        // Act
        var result = parser.ParseFinalTableData(csvLines).ToArray();

        // Assert
        Assert.Single(result);
        var data = result[0];
        Assert.Equal("1", data.Lp);
        Assert.Equal("9", data.BuildingNr);
        Assert.Equal("0", data.StageNr);
        Assert.Equal("1-9/0", data.CalculatedPointNr);
        Assert.Equal("+2,40m", data.AdditionalHeight); // CsvHelper properly handles quoted fields
        Assert.Equal("2", data.AppartmentNr);
        Assert.Equal("pokój nr 4", data.RoomType);
        Assert.Equal("pora dzienna", data.DayOrNight);
    }

    [Fact]
    public void ParseFinalTableData_WithMultipleRows_ParsesAll()
    {
        // Arrange
        var csvLines = new[]
        {
            "L.p.,Nr budynku,Numer kondygnacji,Numer punktu obliczeniowego,Wysokość Z n.p.t. [m],Nr lokalu,Rodzaj pomieszczenia - funkcja,Obliczony poziom dźwięku (dB),Column8,Miarodajny poziom dźwięku A hałasu zewnętrznego w dB,Column10,Opis przegrody,Column12,Column13,Column14,Column15,UWAGI,Column17,Column18",
            "1,Building1,0,Point1,2.5m,Apt1,Room1,Day,NoiseVal,Range1,Index1,Partition1,Acoustic1,Value1,Sound1,Value2,Comment1,Sep1,1000;2000",
            "2,Building2,1,Point2,3.0m,Apt2,Room2,Night,NoiseVal2,Range2,Index2,Partition2,Acoustic2,Value3,Sound2,Value4,Comment2,Sep2,2000;3000"
        };

        // Act
        var result = parser.ParseFinalTableData(csvLines).ToArray();

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal("1", result[0].Lp);
        Assert.Equal("2", result[1].Lp);
        Assert.Equal("Building1", result[0].BuildingNr);
        Assert.Equal("Building2", result[1].BuildingNr);
    }
}
