using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EchotonToolsMauiAppLib.Helpers;

public class DxfParser
{
    private const string FLOATING_NUMBER_REGEX_PATTERN = "[-+]?[0-9]*\\.?[0-9]+";
    private readonly IList<string> sequenceList = [
        "AcDbEntity",
        FLOATING_NUMBER_REGEX_PATTERN,
        "punkt wysoko.*",
        FLOATING_NUMBER_REGEX_PATTERN,
        FLOATING_NUMBER_REGEX_PATTERN,
        FLOATING_NUMBER_REGEX_PATTERN,
        "AcDbText",
        FLOATING_NUMBER_REGEX_PATTERN,
        $"({FLOATING_NUMBER_REGEX_PATTERN})",
        FLOATING_NUMBER_REGEX_PATTERN,
        $"({FLOATING_NUMBER_REGEX_PATTERN})",
        ".*",
        ".*",
        ".*",
        ".*",
        ".*",
        $"({FLOATING_NUMBER_REGEX_PATTERN})",
    ];

    public int Parse(string filePath, string outputPath)
    {
        var inputLines = File.ReadAllLines(filePath);
        var outputLines = Parse(inputLines);
        File.WriteAllLines(
            outputPath,
            outputLines
        );
        return outputLines.Length;
    }

    public string[] Parse(string[] inputLines)
    {
        IList<string> outputLines = [];

        var sequenceIdx = 0;
        var outputLine = "";

        for (int i = 0; i < inputLines.Length; i++)
        {
            var line = inputLines[i].Trim();
            var sequenceValue = sequenceList[sequenceIdx];
            var match = Regex.Match(line, $"\\s*{sequenceValue}\\s*");

            if (match.Success)
            {
                if (match.Groups.Count > 1)
                {
                    outputLine += match.Groups[1].Value + ",";
                }
                if (sequenceIdx + 1 == sequenceList.Count)
                {
                    sequenceIdx = 0;
                    outputLines.Add(outputLine.Substring(0, outputLine.Length - 1));
                    outputLine = "";
                }
                else
                {
                    sequenceIdx++;
                }
            } else
            {
                sequenceIdx = 0;
                outputLine = "";
            }
        }
        return outputLines.ToArray();
    }
}