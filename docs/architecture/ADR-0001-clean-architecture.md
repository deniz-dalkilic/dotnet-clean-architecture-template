# ADR-0001: Clean Architecture baseline and dependency rule

## Status
Accepted

## Context
This template needs a simple, explicit architecture baseline that scales from small features to larger modules.

## Decision
We adopt Clean Architecture with the dependency rule:

- **Domain** contains enterprise/business rules and does not depend on other layers.
- **Application** contains use-cases and abstractions, depending only on Domain.
- **Infrastructure** implements Application abstractions and external integrations.
- **Api/Worker** are composition and delivery mechanisms.

Dependencies point inward only.

## Why no mediator by default
We intentionally avoid mediator plumbing for baseline use-cases.

- Direct services are easier to discover and debug.
- Fewer abstractions reduce cognitive overhead for small teams.
- Explicit constructor dependencies keep use-cases testable without framework indirection.

If the project grows and requires request pipelines, mediator can be introduced intentionally as an implementation detail.

## Consequences
- New features should be added as explicit use-case services in `Application/UseCases/...`.
- Cross-cutting concerns should be implemented with clear abstractions (`IClock`, `ICurrentUser`, `ICache`, etc.) and realized in Infrastructure.
- Layering and references must continue to enforce inward dependency flow.
