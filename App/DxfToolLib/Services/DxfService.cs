using netDxf.Header;
using netDxf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using DxfToolLib.Schemas;
using DxfToolLib.Schemas.Core;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.ComponentModel;
using DxfToolLib.Models;
using DxfToolLib.Services;
using DxfToolLib.Extensions;

namespace DxfToolLib.Helpers;

internal class DxfService : IDxfService
{
    private readonly ISchemaFinder schemaFinder;
    private readonly IGpsCoordsFinder gpsCoordsFinder;
    private readonly ICsvParser csvParser;
    private readonly ILogger<DxfService>? logger;

    public DxfService(ISchemaFinder schemaFinder, IGpsCoordsFinder gpsCoordsFinder, ICsvParser csvParser, ILogger<DxfService>? logger = null)
    {
        this.schemaFinder = schemaFinder;
        this.gpsCoordsFinder = gpsCoordsFinder;
        this.csvParser = csvParser;
        this.logger = logger;
        
        logger?.LogDebug("DxfService initialized");
    }

    private static DxfDocument? LoadUsingNetDxf(string filePath)
    {
        var dxfVersion = DxfDocument.CheckDxfFileVersion(filePath);
        // netDxf is only compatible with AutoCad2000 and higher DXF versions
        if (dxfVersion < DxfVersion.AutoCad2000) return null;
        // load file
        return DxfDocument.Load(filePath);
    }

    private string[] FindPointWithMultiLeaderSchema(string[] dxfInputLines, string[] soundPlanLines)
    {
        var schemaKey = KnownSchemas.PointWithMultiLeader;
        var schemaName = schemaKey.NAME;
        var dictionary = new Dictionary<string, string> { };
        var data = schemaFinder.Matches(schemaName, dictionary, dxfInputLines);
        var dxfInSoundPlanFormat = data.Select(itemData =>
        {
            var x = itemData[0];
            var y = itemData[1];
            var z = itemData[2];
            var layer = itemData[3];
            var description = itemData[4];

            return new DxfPoint
            {
                Latitude = x,
                Longitude = y,
                Height = z,
                Layer = layer,
                Description = description,
            };
        });
        var soundPlanPoint = soundPlanLines.MapDxfToSoundPlan();
        
        // Use the dedicated mapper to match DXF points with SoundPlan data
        return DxfSoundPlanMapper.MatchAndFormat(dxfInSoundPlanFormat, soundPlanPoint);
    }


    private string[] FindHighPointsSchema1(string dxfHighPointName, string[] inputLines)
    {
        var schemaKey = KnownSchemas.HighPointAutoCad2000;
        var schemaName = schemaKey.NAME;
        var dictionary = new Dictionary<string, string> {
            { schemaKey.FIELDS.TITLE, dxfHighPointName },
        };
        return schemaFinder.Matches(schemaName, dictionary, inputLines).CombineMatches();
    }

    private string[] FindHighPointsSchema2(string dxfHighPointName, string[] inputLines)
    {
        var schemaKey = KnownSchemas.HighPointAutoCad2004;
        var schemaName = schemaKey.NAME;
        var dictionary = new Dictionary<string, string> {
            { schemaKey.FIELDS.TITLE, dxfHighPointName },
        };
        return schemaFinder.Matches(schemaName, dictionary, inputLines).CombineMatches();
    }

    public string[] FindHighPoints(int dxfVersion, string dxfHighPointName, string[] inputLines)
    {
        var matches1 = FindHighPointsSchema1(dxfHighPointName, inputLines);
        var matches2 = FindHighPointsSchema2(dxfHighPointName, inputLines);
        if (matches1.Length > 0)
        {
            return matches1;
        }
        else
        {
            return matches2;
        }
    }

    public string[] FindHighPoints(int dxfVersion, string[] inputLines)
    {
        // Find high points without specifying a name (use empty string to match all)
        return FindHighPoints(dxfVersion, "", inputLines);
    }

    public Encoding GetEncoding(string[] inputLines, Encoding defaultEncoding)
    {
        var matches = schemaFinder.Matches(KnownSchemas.CodePage.NAME, null, inputLines).CombineMatches();
        var encodingName = matches.FirstOrDefault();

        if (encodingName == null)
        {
            return defaultEncoding;
        }
        var encodingText = encodingName.Split('_').Where((el, idx) => idx == 1).FirstOrDefault();
        if (encodingText == null)
        {
            return defaultEncoding;
        }
        if (Int32.TryParse(encodingText, out int encodingCodePage))
        {
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(encodingCodePage);
            if (encoding != null)
            {
                return encoding;
            }
        }
        return defaultEncoding;
    }

    public int GetVersion(string[] inputLines, int defaultValue = 0)
    {
        var matches = schemaFinder.Matches(KnownSchemas.CadVersion.NAME, null, inputLines).CombineMatches();
        var versionName = matches.FirstOrDefault();
        if (versionName == null)
        {
            return defaultValue;
        }
        if (Int32.TryParse(versionName.AsSpan(2), out int version))
        {
            return version;
        }
        return defaultValue;
    }

    public int OperateOnAFile(string filePath, string outputPath, Func<int, string[], string[]> function)
    {
        logger?.LogInformation("Starting file operation - Input: {InputFile}, Output: {OutputFile}", filePath, outputPath);
        
        try
        {
            var header = File.ReadLines(filePath).Take(20).ToArray();
            var dxfEncoding = GetEncoding(header, Encoding.Default);
            var inputLines = File.ReadAllLines(filePath, dxfEncoding);
            var dxfVersion = GetVersion(header);

            logger?.LogDebug("File loaded - Version: {Version}, Encoding: {Encoding}, Lines: {LineCount}", 
                dxfVersion, dxfEncoding.EncodingName, inputLines.Length);

            var outputLines = function(dxfVersion, inputLines);

            File.WriteAllLines(outputPath, outputLines);
            
            logger?.LogInformation("File operation completed - Output lines: {OutputCount}", outputLines.Length);
            return outputLines.Length;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "File operation failed - Input: {InputFile}, Output: {OutputFile}", filePath, outputPath);
            throw;
        }
    }

    public int FindHighPoints(string dxfHighPointName, string filePath, string outputPath)
    {
        //if (dxfVersion > 1015)
        //{
        //    //var dxfDocument = LoadUsingNetDxf(filePath);
        //    //if (dxfDocument != null)
        //    //{
        //    //    Debug.WriteLine(dxfDocument.Entities.Texts.Count() + "");
        //    //}
        //    throw new Exception("Ta wersja AutoCad nie jest obsługiwana");
        //}
        //else
        //{
        return OperateOnAFile(filePath, outputPath, (dxfVersion, input) =>
        {
            return FindHighPoints(dxfVersion, dxfHighPointName, input);
        });
    }

    public int FindHighPoints(string filePath, string outputPath)
    {
        return OperateOnAFile(filePath, outputPath, (dxfVersion, input) =>
        {
            return FindHighPoints(dxfVersion, input);
        });
    }

    public int FindAllGpsCoords(string filePath, string outputPath)
    {
        return OperateOnAFile(filePath, outputPath, (dxfVersion, input) =>
        {
            if (dxfVersion > 1015)
            {
                var coords = gpsCoordsFinder.Find(input, KnownGpsCoords.Poland.Min, KnownGpsCoords.Poland.Max);
                return [.. coords.Select(coords => $"{coords.Longitude},{coords.Latitude},{coords.Height}")];
            }
            throw new Exception($"Tej wersji AutoCad {dxfVersion} nie obsługujemy. Program wspiera wersje do 1015 włącznie");
        });
    }

    public int FindPointsWithMultiLeadersSave(string dxfFilePath, string soundPlanFilePath, string finalTableCsvFilePath, string outputPath)
    {
        var soundPlanLines = File.ReadLines(soundPlanFilePath).ToArray();
        
        // Parse CSV using CsvHelper - much more robust than our custom parser
        var finalTableData = csvParser.ParseFinalTableDataFromFile(finalTableCsvFilePath).ToArray();
        
        logger?.LogInformation("Parsed {Count} final table data entries from CSV using CsvHelper", finalTableData.Length);
        
        return OperateOnAFile(dxfFilePath, outputPath, (dxfVersion, input) =>
        {
            return FindPointsWithMultiLeaders(dxfVersion, input, soundPlanLines, finalTableData);
        });
    }

    public string[] FindPointsWithMultiLeaders(int dxfVersion, string[] inputLines, string[] soundPlanLines)
    {
        return FindPointWithMultiLeaderSchema(inputLines, soundPlanLines);
    }

    public string[] FindPointsWithMultiLeaders(int dxfVersion, string[] inputLines, string[] soundPlanLines, FinalTableData[] finalTableData)
    {
        // For now, use the existing method - you can extend this to use finalTableData as needed
        var result = FindPointWithMultiLeaderSchema(inputLines, soundPlanLines);
        
        // Log the final table data for debugging
        logger?.LogDebug("Final table data contains {Count} entries", finalTableData.Length);
        
        // Example of how to work with the parsed CSV data:
        var validEntries = finalTableData
            .WithValidLp()                    // Filter entries with valid sequential numbers
            .WithCalculatedPointNumbers()     // Filter entries with valid calculation point numbers
            .WithCoordinates()               // Filter entries with valid coordinates
            .ToArray();

        // Fill missing cells by copying data from previous complete rows
        var filledEntries = validEntries.FillFromPreviousRows();
        var selectedCsvEntries = filledEntries.Select(e => new
        {
            e.AdditionalHeight,
            e.CalculatedPointNr,
            x = e.Coordinates.Split(';').ElementAtOrDefault(0)?.Trim(),
            y = e.Coordinates.Split(';').ElementAtOrDefault(1)?.Trim()
        }).ToArray();
        logger?.LogInformation("Found {ValidCount} valid entries with coordinates out of {TotalCount} total entries, filled {FilledCount} entries", 
            validEntries.Length, finalTableData.Length, filledEntries.Length);
        
        // TODO: Implement logic to combine DXF points, SoundPlan data, and FinalTableData
        // This is where you would implement the business logic to match and merge the data
        // For example:
        // 1. Match DXF points with FinalTableData entries by coordinates or point numbers
        // 2. Combine the acoustic calculation data from FinalTableData with spatial data from DXF
        // 3. Format the output according to your requirements
        
        return result;
    }
}