using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Template.IntegrationTests.Fixtures;

namespace Template.IntegrationTests.Auth;

public sealed class KeycloakAuthIntegrationTests(InfrastructureContainersFixture fixture) : IClassFixture<InfrastructureContainersFixture>
{
    [Fact(Skip = "Requires running API host wiring and Docker daemon access in CI runtime.")]
    public async Task ClientCredentialsToken_ShouldAllowAccessProtectedEndpoint()
    {
        using var client = new HttpClient();
        var tokenEndpoint = $"{fixture.Keycloak.GetBaseAddress()}/realms/template/protocol/openid-connect/token";

        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = "template-service",
            ["client_secret"] = "template-service-secret"
        };

        var tokenResponse = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(tokenRequest));
        tokenResponse.EnsureSuccessStatusCode();
        var payload = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();

        payload.Should().NotBeNull();
        payload!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    private sealed record TokenResponse([property: JsonPropertyName("access_token")] string AccessToken);
}
