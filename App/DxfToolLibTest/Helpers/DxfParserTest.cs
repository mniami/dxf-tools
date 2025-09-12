namespace DxfToolLibTest;

using DxfToolLib.Services;
using DxfToolLib.Schemas;
using DxfToolLib.Schemas.Core;
using Xunit;

public class DxfServiceTest
{
    private readonly IDxfService parser;

    public DxfServiceTest(IDxfService parser) => this.parser = parser;

    [Theory]
    [InlineData("Warstwa 27", @"
    ATTRIB
  5
3D7B
330
20BE
100
AcDbEntity
  8
Warstwa 27
  6
Continuous
 62
     7
370
     0
100
AcDbText
 10
6506054.241000001
 20
5886993.547000001
 30
0.0
 40
0.75
  1
43.16 
  7
Style-ITALIC_1250
    ", "6506054.241000001,5886993.547000001,43.16")]
    [InlineData("Warstwa 27", @"
835
330
32DA
100
AcDbEntity
  8
Warstwa 27
  6
Continuous
 62
     7
370
     0
100
AcDbText
 10
0.5000000004656613
 20
-0.375
 30
0.0
 40
0.75
  1
49.22
  7
Style-ITALIC_1250", "0.5000000004656613,-0.375,49.22")]
    [InlineData("punkt wysoko�ciowy", @"
AcDbEntity
2E8
punkt wysoko�ciowy sztuczny-Atr2
 100
7
  8
AcDbEntity
9
punkt wysoko�ciowy sztuczny-Atr2
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
    [InlineData("punkt wysoko�ciowy", @"2E8
 100
AcDbEntity
  8
punkt wysoko�ciowy sztuczny-Atr2
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
        var outputLines = parser.FindHighPoints(1014, highPointName, input.Split('\n'));
        Assert.Equal(expectedOutput, string.Join('\n', outputLines));
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData("punkt wysokościowy", "Resources\\jeżewo.dxf")]
    public Task ParseFileTest(string highPointName, string filePath)
    {
        var inputLines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
        var outputLines = parser.FindHighPoints(1014, highPointName, inputLines);
        
        return Verify(outputLines);
    }

    [Theory]
    [InlineData("Resources\\geometry-point.dxf")]
    public Task ParsePointsWithMultiLeadersTest(string filePath)
    {
        var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", filePath);
        var inputLines = File.ReadAllLines(fullPath, System.Text.Encoding.UTF8);

        // Use FindHighPoints to maintain compatibility with existing test expectations
        var outputLines = parser.FindHighPoints(1014, "", inputLines);
        return Verify(outputLines);
    }
}