namespace DxfToolLibTest;

using DxfToolLib.Helpers;
using DxfToolLib.Schemas;
using Xunit;

public class DxfParserTest
{
    readonly DxfParser parser = new(new SchemaStorage([
        new HighPointAutoCad2000Schema(),
    ]));

    [Theory]
    [InlineData("punkt wysokoœciowy", @"2E8
 100
AcDbEntity
  8
punkt wysokoœciowy sztuczny-Atr2
 62
7
 100
AcDbText
 10
6533599.54
 20
5931087.52
 40
0.75
  1
78.88
 50
9.032
 41
0.9
 51
0.0
 72
0
 100
AcDbText", @"6533599.54,5931087.52,9.032")]
    public Task ParseTest(string highPointName, string input, string expectedOutput)
    {
        var outputLines = parser.Parse(highPointName, input.Split('\n'));
        Assert.Equal(expectedOutput, string.Join('\n', outputLines));
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData("punkt wysokoœciowy", "Resources\\je¿ewo.dxf")]
    public Task ParseFileTest(string highPointName, string filePath)
    {
        var inputLines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
        var outputLines = parser.Parse(highPointName, inputLines);
        
        return Verify(outputLines);
    }
}