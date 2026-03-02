namespace Template.Infrastructure.Caching;

public sealed class RedisCacheOptions
{
    public const string SectionName = "Redis";
    public string Configuration { get; init; } = "localhost:6379";
}
