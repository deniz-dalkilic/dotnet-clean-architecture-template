using FluentAssertions;
using Template.Application.Features.Health;

namespace Template.UnitTests.Application;

public sealed class GetHealthServiceTests
{
    [Fact]
    public void Execute_ShouldReturnOkStatus()
    {
        var sut = new GetHealthService();

        var result = sut.Execute();

        result.Status.Should().Be("ok");
        result.UtcTimestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }
}
