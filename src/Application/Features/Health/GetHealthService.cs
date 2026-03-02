namespace Template.Application.Features.Health;

public sealed class GetHealthService
{
    public HealthResponse Execute() => new("ok", DateTimeOffset.UtcNow);
}

public sealed record HealthResponse(string Status, DateTimeOffset UtcTimestamp);
