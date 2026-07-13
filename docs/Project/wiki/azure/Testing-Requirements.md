# Testing Requirements (MCP Server)

## TEST-CORE

### TEST-CORE-001

xUnit suite (24 tests, serialized assembly) covering GatewayMapper route/cluster mapping and prefix/upstream validation, LiteDbRouteStore seeding (idempotent, fill-only-empty), and WAF endpoints - landing lists networks, health anonymous, route API 401, admin redirects to login, configured prefix proxies (502 with no upstream), unknown prefix 404, login without Google config 503. Run dotnet test EternalSocial.slnx -c Debug; gate is 100 percent green, zero skipped.
