using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DxfToolLib.Helpers
{
    internal class MatchesFinder : IMatchesFinder
    {
        public string[][] GetFoundMatches(IList<string> items, string[] input)
        {
            IList<string[]> outputResults = [];

            var sequenceIdx = 0;
            var outputLine = new List<string> { };

            for (int i = 0; i < input.Length; i++)
            {
                var line = input[i].Trim();
                var sequenceValue = items[sequenceIdx];
                var match = Regex.Match(line, $"\\s*{sequenceValue}\\s*");

                if (match.Success)
                {
                    if (match.Groups.Count > 1)
                    {
                        outputLine.Add(match.Groups[1].Value);
                    }
                    if (sequenceIdx + 1 == items.Count)
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
                    if (sequenceIdx > 0)
                    {
                        i -= sequenceIdx;
                    }
                    sequenceIdx = 0;
                    outputLine = [];
                }
            }
            return [.. outputResults];
        }
    }
}
