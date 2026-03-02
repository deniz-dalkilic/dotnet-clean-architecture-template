using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Template.IntegrationTests.Fixtures;

public sealed class InfrastructureContainersFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgreSql { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("template")
        .WithUsername("template")
        .WithPassword("template")
        .Build();

    public RedisContainer Redis { get; } = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .Build();

    public KeycloakContainer Keycloak { get; } = new KeycloakBuilder()
        .WithImage("quay.io/keycloak/keycloak:26.0")
        .WithRealmImportFile("infra/keycloak/realm/template-realm.json")
        .Build();

    public async Task InitializeAsync()
    {
        await PostgreSql.StartAsync();
        await Redis.StartAsync();
        await Keycloak.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Keycloak.DisposeAsync();
        await Redis.DisposeAsync();
        await PostgreSql.DisposeAsync();
    }
}
