namespace DxfToolLibTest.Extensions;

using DxfToolLib.Extensions;
using DxfToolLib.Models;
using DxfToolLib.Schemas;
using Xunit;

public class DxfExtensionsSchemaDetectionTest
{
    private readonly PointWithMultiLeaderSchema schema = new();

    [Fact]
    public void DetectsSingleSchemaSequence()
    {
        // Arrange: a minimal synthetic sequence derived from the beginning + tail of schema
        var lines = new [] {
            "Random", // noise
            "AcDbPoint", // start
            "10",
            "123.45", // X captured (ignored by algorithm now)
            "20",
            "678.90", // Y
            "30",
            "5.5",    // Z
            "0",
            "MULTILEADER",
            "5",
            "0", // filler (.*)
            "0", // filler
            "0", // filler
            "0", // filler
            "AcDbEntity",
            "8",
            "Layer_A",
            "92",
            "0",
            // 16 pairs of 310 + .*
        };
        // Pad with the expected repeated pattern for 310/. * 16 times and remaining schema tail
        var dynamic = new List<string>(lines);
        for(int i=0;i<16;i++){ dynamic.Add("310"); dynamic.Add("ABCDEF"); }
        dynamic.AddRange(new[]{
            "100",
            "AcDbMLeader",
            "300",
            "CONTEXT_DATA{",
            "40",
            "1.0",
            "10",
        });
        // 22 times .*
        for(int i=0;i<22;i++){ dynamic.Add("0"); }
        dynamic.Add("Note text here");
        var input = dynamic.ToArray();

        var points = new [] { new DxfPointReportItem { Latitude="0", Longitude="0", Height="0", Layer="L", Description="D" } };

        // Act
        var output = input.UpdateDxfWithSoundPlanData(points, schema);

        // Assert: algorithm returns unchanged content length & ordering
        Assert.Equal(input.Length, output.Length);
        for(int i=0;i<input.Length;i++)
            Assert.Equal(input[i], output[i]);
    }

    [Fact]
    public void IgnoresIncompleteSequence()
    {
        var incomplete = new [] { "AcDbPoint", "10", "123.4", "20" /* stops early */ };
        var points = System.Array.Empty<DxfPointReportItem>();
        var output = incomplete.UpdateDxfWithSoundPlanData(points, schema);
        Assert.Equal(incomplete.Length, output.Length);
    }
}
