# Template

A pragmatic .NET clean architecture template with explicit use-cases and infrastructure boundaries.

## Project structure

- `src/Domain`
  - Core business model and primitives (`Entity<TId>`, `DomainEvent`, `Result`).
  - No dependency on other project layers.
- `src/Application`
  - Use-cases and application contracts/ports (`IAppDbContext`, `IClock`, `ICurrentUser`, `IUnitOfWork`, `ICache`, `IEventBus`).
  - Depends only on Domain.
- `src/Infrastructure`
  - Adapters and implementations for persistence, jobs, messaging, auth, and cache.
  - Implements Application abstractions.
- `src/Api`
  - HTTP delivery layer and composition root.
- `src/Worker`
  - Background processing host.
- `tests/UnitTests`
  - Fast unit tests for domain/application logic.
- `tests/IntegrationTests`
  - Integration tests for infrastructure and external dependencies.
- `docs/architecture`
  - Architecture notes and ADRs.

## Architectural direction

See `docs/architecture/ADR-0001-clean-architecture.md` for the baseline dependency rule and rationale for keeping use-cases explicit without mediator by default.

## Local infrastructure with Docker Compose

1. Copy the sample environment file and adjust values if needed:

```bash
cp infra/.env.example infra/.env
```

2. Start core dependencies:

```bash
docker compose --env-file infra/.env -f infra/docker-compose.core.yml up -d
```

3. Optionally add observability stack:

```bash
docker compose --env-file infra/.env -f infra/docker-compose.core.yml -f infra/docker-compose.observability.yml up -d
```

## Authentication approach

This template does not include a bundled identity provider. Use an external OpenID Connect/OAuth 2.0 provider and apply an **External Id Token Exchange** pattern for service-to-service calls. See `docs/architecture/authentication.md` for details.
