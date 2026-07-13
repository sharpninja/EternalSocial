# EternalSocial

The gateway for the Eternal estate: a YARP reverse proxy that fronts
EternalReadit (`/r`), EternalX (`/x`), and EternalDiscord (`/d`) on one docker
network, owns the single Google OIDC sign-in, and shares the identity with every
site it proxies (see [docs/gateway-sso.md](docs/gateway-sso.md)).

- `src/EternalSocial.Proxy` - the gateway: data-driven routes (LiteDB), landing
  page, request logs, owner-only admin console, identity-forwarding transform.
- `tests/EternalSocial.Proxy.Tests` - xUnit suite (mapper, route store, endpoints).
- `deploy/octopus-deploy.ps1` - git-sourced Octopus step: builds and runs the
  gateway container and the ngrok tunnel that fronts it.

Build and test:

```
dotnet test EternalSocial.slnx
```
