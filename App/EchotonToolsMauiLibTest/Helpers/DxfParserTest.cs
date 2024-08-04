namespace EchotonToolsMauiAppTest;

using EchotonToolsMauiAppLib.Helpers;

public class DxfParserTest
{
    DxfParser parser = new DxfParser();

    [Theory]
    [InlineData(@"2E8
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
    public void ParseTest(string input, string expectedOutput)
    {
        var outputLines = parser.Parse(input.Split('\n'));
        Assert.Equal(expectedOutput, string.Join('\n', outputLines));
    }

    [Theory]
    [InlineData("Resources\\je¿ewo.dxf")]
    public Task ParseFileTest(string filePath)
    {
        var inputLines = File.ReadAllLines(filePath);
        var outputLines = parser.Parse(inputLines);
        
        return Verify(outputLines);
    }
}