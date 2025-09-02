using DxfToolLib.Helpers;
using Xunit;

namespace DxfToolLibTest.Helpers;

public class SoundPlanGpsMapperTest
{
    [Theory]
    [InlineData("5885728.661708554", "5885728,662")]
    [InlineData("123.456789", "123,457")]
    [InlineData("1000.1", "1000,100")]
    [InlineData("0.999", "0,999")]
    [InlineData("0.9999", "1,000")]
    [InlineData("42", "42,000")]
    public void ToSoundPlanGpsCoordinateFormat_ShouldFormatCorrectly(string input, string expected)
    {
        // Act
        var result = input.ToSoundPlanGpsCoordinateFormat();

        // Assert
        Assert.Equal(expected, result);
    }
}
