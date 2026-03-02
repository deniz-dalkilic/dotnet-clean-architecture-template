# Monolith vs Microservices
The same clean architecture boundaries support both modular monolith and microservice deployments:
- Start monolith-first with API + Worker + shared database.
- Extract services per bounded context over time while keeping Domain/Application patterns unchanged.
