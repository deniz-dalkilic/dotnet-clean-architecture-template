# Authentication and Authorization
- Keycloak is the single IdP.
- API uses JWT Bearer authentication with Authority/Audience and configurable role claim mapping.
- Service-to-service tokens use explicit `ServiceTokenProvider` with client credentials grant.
