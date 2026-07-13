# Technical Requirements (MCP Server)

## TR-CORE-ARCH-001

**YARP in-memory routing** — Gateway loads YARP config from memory (InMemoryConfigProvider.Update on route changes). Routes map prefix to '{prefix}/{**catch-all}' at Order 10 with trailing-slash cluster addresses; prefixes pass through un-stripped and downstream apps absorb them with UsePathBase plus a rewritten base href.
**Covered by:** FR: FR-CORE-001, FR-CORE-003; TEST: TEST-CORE-001
**Status:** completed
Scope: layer-1+

## TR-CORE-ARCH-002

**Footer injection transform** — A YARP response transform buffers 200 text/html responses without a Content-Encoding, replaces the first FooterInjector.Marker occurrence with the footer markup, and rewrites the body with a corrected Content-Length. Non-HTML, non-200, compressed, and marker-less documents pass through unchanged; Razor component templates must emit the marker via MarkupString because the Razor compiler strips literal HTML comments.
**Covered by:** FR: FR-CORE-005; TEST: TEST-CORE-001
**Status:** completed
Scope: layer-1+

## TR-CORE-SEC-001

**Identity header trust boundary** — The gateway strips inbound X-Auth-* and X-Gateway-Key from clients, then injects X-Gateway-Key plus X-Auth-UserId/Name/Email from the authenticated principal. Sites accept the headers only when their GATEWAY_KEY config matches; otherwise the request is anonymous. Trust boundary = docker network + shared key.
**Covered by:** FR: FR-CORE-002; TEST: TEST-CORE-001
**Status:** completed
Scope: layer-1+

## TR-CORE-SEC-002

**Forwarded headers and challenge scheme** — ForwardedHeaders options must .Clear() KnownIPNetworks/KnownProxies (collection initializers do not clear defaults - caused http redirect_uri). Gateway rewrites scheme before proxying so downstream ForwardLimit=1 works. DefaultChallengeScheme stays Cookies so APIs return 401 instead of redirecting to Google.
**Covered by:** FR: FR-CORE-002, FR-CORE-004; TEST: TEST-CORE-001
**Status:** completed
Scope: layer-1+

## TR-CORE-SEC-003

**Site container isolation** — Site containers (eternalreddit, eternalx, eternaldiscord) expose no public ports on the docker host. Only the gateway is reachable (host 8090 behind the ngrok tunnel), so all site traffic and identity headers flow through the gateway trust boundary.
**Covered by:** FR: FR-CORE-001; TEST: TEST-CORE-001
**Status:** completed
Scope: layer-1+

