using DxfToolLib.Models;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DxfToolLib.Services;

public interface ICsvParser
{
    IEnumerable<FinalTableData> ParseFinalTableData(string[] csvLines);
    IEnumerable<FinalTableData> ParseFinalTableDataFromFile(string filePath);
}

public class CsvParser : ICsvParser
{
    public IEnumerable<FinalTableData> ParseFinalTableData(string[] csvLines)
    {
        if (csvLines == null || csvLines.Length == 0)
            return Enumerable.Empty<FinalTableData>();

        var csvContent = string.Join(Environment.NewLine, csvLines);
        using var reader = new StringReader(csvContent);
        return ParseFromReader(reader);
    }

    public IEnumerable<FinalTableData> ParseFinalTableDataFromFile(string filePath)
    {
        using var reader = new StreamReader(filePath);
        return ParseFromReader(reader).ToList(); // ToList() to ensure the reader isn't disposed before enumeration
    }

    private IEnumerable<FinalTableData> ParseFromReader(TextReader reader)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null, // Don't throw on missing fields
            HeaderValidated = null,   // Don't validate headers
            PrepareHeaderForMatch = args => args.Header.Trim(),
        };

        using var csv = new CsvReader(reader, config);
        
        // Register the class map for FinalTableData
        csv.Context.RegisterClassMap<FinalTableDataMap>();
        
        try
        {
            return csv.GetRecords<FinalTableData>().ToList();
        }
        catch (Exception)
        {
            // If parsing with headers fails, return empty list
            // In practice, you might want to implement fallback logic differently
            return new List<FinalTableData>();
        }
    }
}

// Class map for mapping CSV columns by header name
public class FinalTableDataMap : ClassMap<FinalTableData>
{
    public FinalTableDataMap()
    {
        Map(m => m.Lp).Name("L.p.", "Lp");
        Map(m => m.BuildingNr).Name("Nr budynku", "BuildingNr", "Building Number");
        Map(m => m.StageNr).Name("Numer kondygnacji", "StageNr", "Stage Number");
        Map(m => m.CalculatedPointNr).Name("Numer punktu obliczeniowego", "CalculatedPointNr", "Calculated Point Number");
        Map(m => m.AdditionalHeight).Name("Wysokość Z n.p.t. [m]", "AdditionalHeight", "Additional Height");
        Map(m => m.AppartmentNr).Name("Nr lokalu", "AppartmentNr", "Apartment Number");
        Map(m => m.RoomType).Name("Rodzaj pomieszczenia - funkcja", "RoomType", "Room Type");
        Map(m => m.DayOrNight).Name("Obliczony poziom dźwięku (dB)", "DayOrNight", "Day Or Night");
        Map(m => m.CalculatedNoiseValue).Index(8);
        Map(m => m.MaxNoiseRange).Name("Miarodajny poziom dźwięku A hałasu zewnętrznego w dB", "MaxNoiseRange");
        Map(m => m.AssessmentIndex).Index(10);
        Map(m => m.PartitionDescription).Name("Opis przegrody", "PartitionDescription");
        Map(m => m.RequiredAcousticIsoInsulationType).Index(12);
        Map(m => m.RequiredAcousticInsulationValue).Index(13);
        Map(m => m.RequiredSoundInsulationType).Index(14);
        Map(m => m.RequiredSoundInsulationValue).Index(15);
        Map(m => m.Comments).Name("UWAGI", "Comments");
        Map(m => m.Separator).Index(17);
        Map(m => m.Coordinates).Index(18);
    }
}

// Class map for mapping CSV columns by index (when no headers)
public class FinalTableDataMapByIndex : ClassMap<FinalTableData>
{
    public FinalTableDataMapByIndex()
    {
        Map(m => m.Lp).Index(0);
        Map(m => m.BuildingNr).Index(1);
        Map(m => m.StageNr).Index(2);
        Map(m => m.CalculatedPointNr).Index(3);
        Map(m => m.AdditionalHeight).Index(4);
        Map(m => m.AppartmentNr).Index(5);
        Map(m => m.RoomType).Index(6);
        Map(m => m.DayOrNight).Index(7);
        Map(m => m.CalculatedNoiseValue).Index(8);
        Map(m => m.MaxNoiseRange).Index(9);
        Map(m => m.AssessmentIndex).Index(10);
        Map(m => m.PartitionDescription).Index(11);
        Map(m => m.RequiredAcousticIsoInsulationType).Index(12);
        Map(m => m.RequiredAcousticInsulationValue).Index(13);
        Map(m => m.RequiredSoundInsulationType).Index(14);
        Map(m => m.RequiredSoundInsulationValue).Index(15);
        Map(m => m.Comments).Index(16);
        Map(m => m.Separator).Index(17);
        Map(m => m.Coordinates).Index(18);
    }
}
