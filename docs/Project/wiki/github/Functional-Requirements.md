# Functional Requirements (MCP Server)

## FR-CORE-001 EternalSocial gateway estate

A standalone YARP gateway fronts EternalReadit (/r), EternalX (/x), and EternalDiscord (/d) on one docker network with a landing page listing the networks, request logs, and an owner-only admin console.
Scope: layer-1+

## FR-CORE-002 Single sign-on shared to all sites

The gateway owns the single Google OIDC sign-in at the public root and forwards identity to every proxied site via X-Auth-UserId/Name/Email plus X-Gateway-Key; sites build their principal from those headers only when GATEWAY_KEY matches and never run their own OAuth in gateway mode.
Scope: layer-1+

## FR-CORE-003 Admin-configurable proxy routes

Proxy prefixes are data-driven (LiteDB) with CRUD from the admin console, prefix validation against reserved paths, enable/disable, and a coming-soon page for enabled routes with no upstream. Seeding fills empty upstreams with defaults but never overwrites admin edits.
Scope: layer-1+

## FR-CORE-004 Persistent sign-in

Logged-in users stay logged in across visits and restarts - the gateway issues a long-lived (365 day) authentication cookie so sessions survive between uses.
Scope: layer-1+

