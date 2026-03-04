using System.Net;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Template.Infrastructure.Resilience;

public static class HttpResiliencePolicies
{
    public const string RetryPolicyName = "http-retry-with-jitter";
    public const string CircuitBreakerPolicyName = "http-circuit-breaker";
    public const string TimeoutPolicyName = "http-timeout";

    public static IAsyncPolicy<HttpResponseMessage> RetryWithJitterPolicy { get; } =
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), retryCount: 5));

    public static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy { get; } =
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));

    public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy { get; } =
        Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10), TimeoutStrategy.Optimistic);
}
