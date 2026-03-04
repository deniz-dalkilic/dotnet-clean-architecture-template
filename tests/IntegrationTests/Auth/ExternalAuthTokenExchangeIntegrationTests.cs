using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Domain.Entities;
using Template.Infrastructure.Data;
using Template.IntegrationTests.Fixtures;

namespace Template.IntegrationTests.Auth;

public sealed class ExternalAuthTokenExchangeIntegrationTests(ExternalAuthWebApplicationFactory factory)
    : IClassFixture<ExternalAuthWebApplicationFactory>
{
    [Fact]
    public async Task ExternalGoogleTokenExchange_ShouldIssueAccessToken_AllowMeEndpoint_AndAvoidDuplicates()
    {
        using var client = factory.CreateClient();

        var firstResponse = await client.PostAsJsonAsync("/api/auth/external/google", new ExternalSignInRequest("dummy-token", null));

        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var firstPayload = await firstResponse.Content.ReadFromJsonAsync<ExternalSignInResponse>();
        firstPayload.Should().NotBeNull();
        firstPayload!.AccessToken.Should().NotBeNullOrWhiteSpace();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", firstPayload.AccessToken);
        var meResponse = await client.GetAsync("/api/me");
        meResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var users = await dbContext.Users.AsNoTracking().ToListAsync();
            var identities = await dbContext.ExternalIdentities.AsNoTracking().ToListAsync();

            users.Should().HaveCount(1);
            identities.Should().ContainSingle(x =>
                x.Provider == ExternalAuthProvider.Google &&
                x.Subject == "sub-123");
        }

        var secondResponse = await client.PostAsJsonAsync("/api/auth/external/google", new ExternalSignInRequest("dummy-token", null));

        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var secondScope = factory.Services.CreateScope();
        var secondDbContext = secondScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userCount = await secondDbContext.Users.CountAsync();
        var identityCount = await secondDbContext.ExternalIdentities.CountAsync();

        userCount.Should().Be(1);
        identityCount.Should().Be(1);
    }

    private sealed record ExternalSignInRequest(string IdToken, string? Nonce);

    private sealed record ExternalSignInResponse(string AccessToken);
}
