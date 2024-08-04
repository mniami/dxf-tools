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

namespace DxfToolLib.Helpers;

public class DxfParser : IDxfParser
{
    private readonly ISchemaStorage schemaStorage;

    public DxfParser(ISchemaStorage schemaStorage)
    {
        this.schemaStorage = schemaStorage;
    }

    private static DxfDocument? LoadUsingNetDxf(string filePath)
    {
        var dxfVersion = DxfDocument.CheckDxfFileVersion(filePath);
        // netDxf is only compatible with AutoCad2000 and higher DXF versions
        if (dxfVersion < DxfVersion.AutoCad2000) return null;
        // load file
        return DxfDocument.Load(filePath);
    }

    public string[][] GetFoundMatches(IList<string> schemaItems, string[] input)
    {
        IList<string[]> outputResults = [];

        var sequenceIdx = 0;
        var outputLine = new List<string> { };

        for (int i = 0; i < input.Length; i++)
        {
            var line = input[i].Trim();
            var sequenceValue = schemaItems[sequenceIdx];
            var match = Regex.Match(line, $"\\s*{sequenceValue}\\s*");

            if (match.Success)
            {
                if (match.Groups.Count > 1)
                {
                    outputLine.Add(match.Groups[1].Value);
                }
                if (sequenceIdx + 1 == schemaItems.Count)
                {
                    sequenceIdx = 0;
                    outputResults.Add([.. outputLine]);
                    outputLine = [];
                }
                else
                {
                    sequenceIdx++;
                }
            }
            else
            {
                sequenceIdx = 0;
                outputLine = [];
            }
        }
        return [.. outputResults];
    }

    public string[] Parse(string dxfHighPointName, string[] inputLines)
    {
        var schemaItems = schemaStorage.GetSchemaItemsByName(KnownSchemas.HighPointAutoCad2000.NAME, new Dictionary<string, string>{
            { KnownSchemas.HighPointAutoCad2000.FIELDS.TITLE, dxfHighPointName },
        });
        return GetFoundMatches(schemaItems, inputLines)
            .Select(item => string.Join(",", item))
            .ToArray();
    }

    public int Parse(string dxfHighPointName, string filePath, string outputPath)
    {
        var dxfDocument = LoadUsingNetDxf(filePath);
        if (dxfDocument != null)
        {
            Debug.WriteLine(dxfDocument.Entities.Texts.Count() + "");
        }
        var inputLines = File.ReadAllLines(filePath, Encoding.UTF8);
        var outputLines = Parse(dxfHighPointName, inputLines);

        File.WriteAllLines(
            outputPath,
            outputLines
        );
        return outputLines.Length;
    }
}