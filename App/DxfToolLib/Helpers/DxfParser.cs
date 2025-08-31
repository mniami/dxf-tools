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

namespace DxfToolLib.Helpers;

internal class DxfParser : IDxfParser
{
    private readonly ISchemaFinder schemaFinder;
    private readonly IGpsCoordsFinder gpsCoordsFinder;
    private readonly ILogger<DxfParser>? logger;

    public DxfParser(ISchemaFinder schemaFinder, IGpsCoordsFinder gpsCoordsFinder, ILogger<DxfParser>? logger = null)
    {
        this.schemaFinder = schemaFinder;
        this.gpsCoordsFinder = gpsCoordsFinder;
        this.logger = logger;
        
        logger?.LogDebug("DxfParser initialized");
    }

    private static DxfDocument? LoadUsingNetDxf(string filePath)
    {
        var dxfVersion = DxfDocument.CheckDxfFileVersion(filePath);
        // netDxf is only compatible with AutoCad2000 and higher DXF versions
        if (dxfVersion < DxfVersion.AutoCad2000) return null;
        // load file
        return DxfDocument.Load(filePath);
    }

    private string[] FindPointWithMultiLeaderSchema(string[] inputLines)
    {
        var schemaKey = KnownSchemas.PointWithMultiLeader;
        var schemaName = schemaKey.NAME;
        var dictionary = new Dictionary<string, string> {};
        return schemaFinder.Matches(schemaName, dictionary, inputLines);
    }

    private string[] FindHighPointsSchema1(string dxfHighPointName, string[] inputLines)
    {
        var schemaKey = KnownSchemas.HighPointAutoCad2000;
        var schemaName = schemaKey.NAME;
        var dictionary = new Dictionary<string, string> {
            { schemaKey.FIELDS.TITLE, dxfHighPointName },
        };
        return schemaFinder.Matches(schemaName, dictionary, inputLines);
    }

    private string[] FindHighPointsSchema2(string dxfHighPointName, string[] inputLines)
    {
        var schemaKey = KnownSchemas.HighPointAutoCad2004;
        var schemaName = schemaKey.NAME;
        var dictionary = new Dictionary<string, string> {
            { schemaKey.FIELDS.TITLE, dxfHighPointName },
        };
        return schemaFinder.Matches(schemaName, dictionary, inputLines);
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
        var matches = schemaFinder.Matches(KnownSchemas.CodePage.NAME, null, inputLines);
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
        var matches = schemaFinder.Matches(KnownSchemas.CadVersion.NAME, null, inputLines);
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
                return coords.Select(coords => $"{coords.Longitude},{coords.Latitude},{coords.Height}").ToArray();
            }
            throw new Exception($"Tej wersji AutoCad {dxfVersion} nie obsługujemy. Program wspiera wersje do 1015 włącznie");
        });
    }

    public int FindPointsWithMultiLeadersSave(string filePath, string outputPath)
    {
        return OperateOnAFile(filePath, outputPath, (dxfVersion, input) =>
        {
            return FindPointsWithMultiLeaders(dxfVersion, input);
        });
    }

    public string[] FindPointsWithMultiLeaders(int dxfVersion, string[] inputLines)
    {
        return FindPointWithMultiLeaderSchema(inputLines);
    }
}