using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Template.Application.Abstractions;
using Template.Application.Auth;
using Template.Domain.Entities;
using Template.Infrastructure.Data;
using Testcontainers.PostgreSql;

namespace Template.IntegrationTests.Fixtures;

public sealed class ExternalAuthWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("template")
        .WithUsername("template")
        .WithPassword("template")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = _postgreSql.GetConnectionString(),
                ["Serilog:Loki:Endpoint"] = null,
                ["OpenTelemetry:OtlpEndpoint"] = null
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IExternalIdTokenValidator>();
            services.AddSingleton<IExternalIdTokenValidator, FakeExternalIdTokenValidator>();
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSql.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSql.DisposeAsync();
        await base.DisposeAsync();
    }

    private sealed class FakeExternalIdTokenValidator : IExternalIdTokenValidator
    {
        public Task<ExternalIdentityPayload> ValidateAsync(
            ExternalAuthProvider provider,
            string idToken,
            string? expectedNonce,
            CancellationToken cancellationToken)
        {
            var payload = new ExternalIdentityPayload(
                provider,
                "sub-123",
                "demo@example.com",
                "Demo User",
                "test",
                "test",
                DateTimeOffset.UtcNow.AddHours(1),
                EmailVerified: true);

            return Task.FromResult(payload);
        }
    }
}
