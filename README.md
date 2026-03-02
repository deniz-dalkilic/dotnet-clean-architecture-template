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
