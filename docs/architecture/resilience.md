# HTTP resilience for outbound calls

Outbound HTTP calls that are made through `IHttpClientFactory` use reusable Polly policies defined in infrastructure.

## Policies

- **Timeout policy** (`http-timeout`): cancels requests that take longer than 10 seconds.
- **Retry policy** (`http-retry-with-jitter`): retries up to 5 times with decorrelated jittered exponential backoff.
- **Circuit breaker policy** (`http-circuit-breaker`): opens the circuit for 30 seconds after 5 consecutive handled failures.

## What is retried

The retry and circuit-breaker policies handle transient faults only:

- `HttpRequestException` (network/transient transport failures)
- HTTP `5xx`
- HTTP `408 Request Timeout`
- HTTP `429 Too Many Requests`

## What is not retried

The policies do **not** retry non-transient response classes such as:

- HTTP `4xx` client errors other than `408` and `429`
- Logical/business validation failures returned by downstream systems

## Why retry `429` and `5xx`

- `429` generally indicates temporary throttling; backing off with jitter helps reduce synchronized retry storms and gives the provider time to recover capacity.
- `5xx` errors are often temporary server-side failures and can succeed on a subsequent attempt.

## Safety note on retries

Retries should be limited to idempotent operations by default. Do **not** retry non-idempotent requests unless the downstream operation is known to be safe (for example via idempotency keys or guaranteed deduplication).
