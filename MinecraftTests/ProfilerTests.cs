namespace MinecraftApiTests;

public class ProfilerTests
{
    [Fact]
    public void MeasuresAction()
    {
        var x = 18;

        Profiler.MeasureExecutionDuration("ProfilerTests.MeasuresAction", () => { x = 20; });

        x.Should().Be(20);
    }

    [Fact]
    public void MeasuresFunction()
    {
        var x = 18;

        var result = Profiler.MeasureExecutionDuration("ProfilerTests.MeasuresFunction", () =>
        {
            x = 20;

            return 10;
        });

        result.Should().Be(10);

        x.Should().Be(20);
    }
}
