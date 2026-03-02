# Template .NET Clean Architecture

Production-grade .NET 10 template for monolith and microservice projects using Clean Architecture.

## Structure
- `src/Domain` - domain entities and business rules.
- `src/Application` - use cases and contracts (no mediator pattern).
- `src/Infrastructure` - EF Core/Npgsql, Redis, RabbitMQ, Quartz, Keycloak client-credentials provider.
- `src/Api` - HTTP API endpoints and authentication.
- `src/Worker` - optional background worker host.
- `tests/UnitTests` and `tests/IntegrationTests` - xUnit + Testcontainers.
- `infra/*` - local infrastructure stacks.

## Quickstart
1. Start core services:
   - `docker compose -f infra/core/docker-compose.yml up -d`
2. Start Keycloak:
   - `docker compose -f infra/keycloak/docker-compose.yml up -d`
3. Start observability:
   - `docker compose -f infra/observability/docker-compose.yml up -d`
4. Run API:
   - `dotnet run --project src/Api/Template.Api.csproj`
5. Run worker (optional):
   - `dotnet run --project src/Worker/Template.Worker.csproj`

## Identity
- Keycloak is the only IdP used directly by applications.
- Google/Apple/Microsoft should be connected to Keycloak via identity federation.

## Notes
- Redis is default cache. Valkey is included as a commented drop-in option in compose.
- Loki + Console are default Serilog sinks. Seq is intentionally optional.
