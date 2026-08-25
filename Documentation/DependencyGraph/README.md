# Solution Dependency Graph

An interactive dependency graph of the NHS Digital API Platform solution:
project boundaries, per-component method blocks, and colour-coded data flows.

Data and renderer are separate files, all in this folder:

- [graph.yml](./graph.yml) — the manifest: solution name, project list (which
  also names each project's data file), root order, and the event registry.
- `projects/*.yml` — one file per project / package boundary, each declaring
  that project's components with their methods and outbound flows.
- [index.html](./index.html) — the renderer. It fetches the manifest and the
  project files, assembles them, and draws. No build step, but because the
  data is fetched the page must be **served** rather than double-clicked:

```bash
python -m http.server 8731 --bind 127.0.0.1
```

then open `http://127.0.0.1:8731/` — or use the published GitHub Pages copy.

It carries two ways of drawing the same data, switched from the segmented
control in the header:

- **single copy** *(default)* — every component appears exactly once with its
  full method surface, and all consumers' flows converge on it. Best for
  "who touches this?".
- **per consumer** — dependencies are duplicated once per consumer, each copy
  showing only the method rows that consumer uses. Best for "what does this
  one call path actually do?".

The choice lands in the URL (`#single` / `#duplicated`), so a link keeps the
view you were on, and switching carries your current selection across.

## Reading the graph

- **Left → right layering**: SDK entry point → clients → processings →
  orchestrations → foundations → brokers → broker implementations →
  external services.
- **Dashed boxes** are project boundaries. External surfaces show only the
  public members this solution actually calls.
- **Edge colours**:
  - **blue** — direct method call
  - **green** — event publish, **purple** — event subscribe,
    **red** — a publish/subscribe pair in a circular event flow. None appear
    today: this solution has no event bus. The machinery is kept in the
    renderer so an event broker can be modelled later without touching
    `index.html`.
- **Duplication over line-spaghetti** (the *per consumer* view only): a
  dependency is drawn once per consumer, showing only the method rows that
  consumer uses, instead of many lines converging on one shared node. The
  exception is components marked "shared" in the side panel — the external
  surfaces. In the *single copy* view nothing is duplicated, so the `shared`
  flag makes no difference there.
- **Click a method row** to trace that single method's path — the full
  upstream + downstream slice lights up and everything else dims.
- **Click a component header** for the same slice seeded from *every* row of
  that copy at once: the component's whole fan-out, not just its first hop.
  Other copies of the same component stay half-lit so you can find them.
- Whatever is selected is outlined and lettered in **amber**; rows the traced
  path passes through carry a faint blue tint. Click the background or Reset
  to clear. Search finds components and methods. The **utility brokers**
  toggle reveals the DateTime / Identifier / Logging broker copies and the logging external that are hidden by
  default for readability.

At the last scan, 29 declared components and 86 declared edges draw as
**25 components · 79 flows** in the single-copy view and **100 nodes ·
413 flows** per consumer (29 · 86 and 115 · 443 with utility brokers on).

`.github/workflows/pages.yml` publishes this folder to GitHub Pages on every
push to `main` that touches it — `index.html` is the site root. Nothing is
compiled; `index.html`, `graph.yml` and `projects/` are copied as-is. Pages
has to be enabled once in the repository's Settings → Pages (source: GitHub
Actions).

## Current truths captured in the data (scanned 2026-08-11)

- **This is an SDK, not a host.** There is no controller, no worker and no
  database — the whole solution is a class library plus an ASP.NET Core
  companion package. `ApiPlatformClient` is the only front door.
- **`ApiPlatformClient` can be used without a DI container.** The static
  `Create` and the configurations-only constructor build their own
  `ServiceCollection`, register the SDK core, and fall back to the in-memory
  storage brokers — so a console app or a test can new it up directly.
- **`ApiPlatformClientFacade` is dead code.** It is an internal
  `IApiPlatformClient` holding the same two sub-clients, but nothing
  constructs or registers it: `AddApiPlatformSdkCore` registers a hand-built
  `ApiPlatformClient` instead. It shows on the graph with no inbound flows.
- **`LoggingBroker` has no inbound flows on this graph, by design.** Every
  service takes `ILoggingBroker`, but it is only ever reached from the
  `CreateAndLog*` exception factories, and this graph draws happy-path calls
  only. It is a utility broker, so it is hidden behind the toggle along with
  the DateTime and Identifier brokers.
- **Dependency failures are categorised by HTTP status.** Both foundation
  services split `HttpRequestException`: a 4xx becomes a
  `*DependencyValidationException` (the caller sent something the dependency
  rejected), a 5xx or a transport failure becomes a `*DependencyException`.
- **The storage brokers are the extension seam.** `IApiPlatformStateBroker`
  and `IApiPlatformTokenBroker` each have an in-memory implementation in the
  Sdk and a session-backed one in Sdk.AspNetCore. Both are registered with
  `TryAdd`, so whichever the host registers first wins — call
  `AddApiPlatformSdkAspNetCore()` before `AddApiPlatformSdkInMemoryStorage()`
  in a web host, or you get the process-wide singletons.
- **The in-memory brokers are singletons and hold one user's state.** Fine
  for a console app or a test; wrong for a multi-user web host.
- **CIS2 runs without PKCE** — the code says so explicitly; only `client_id`,
  `redirect_uri`, `response_type`, `state` and optional `acr_values` are sent.
- **`GetAccessTokenAsync` refreshes silently** and returns an *empty string*
  rather than throwing when both tokens have expired. The orchestration is
  what turns that into `UnauthorizedPdsOrchestrationException`.
- **PDS responses are never deserialised.** `PdsService` returns the raw FHIR
  JSON string; the `Patient` / `Address` / `PatientLookup` models exist but
  nothing maps onto them.
- **The `ISL.Providers.PDS.*` packages are referenced but unused.** All three
  (`Abstractions`, `FakeFHIR`, `FHIR`) are in the Sdk's `.csproj` and not a
  single `.cs` file mentions them — the PDS call is hand-rolled over
  `IHttpBroker`.
- **`JsonBroker.Serialize` is on the surface but never called.**
- **`ReactApp1.Server` and `reactapp1.client` are empty scaffolding** — no
  source files, not in the `.slnx` — so they are not modelled here.

## Modelling decisions

These are the judgement calls baked into the data files; keep them stable so
successive scans stay comparable.

- **Happy-path calls are drawn; exception-path (`TryCatch` /
  `CreateAndLog*`) logging is NOT.** The `.Validations.cs` partials in this
  solution are pure argument checks with no broker calls, so they contribute
  nothing.
- **Private helpers are attributed to the public method that reaches them** —
  `CareIdentityService.CallbackAsync` carries `ExchangeCodeForTokenAsync`'s
  calls, and `GetAccessTokenAsync` carries
  `ExchangeRefreshTokenForTokenAsync`'s. No component links to itself.
- **A swappable interface is drawn once with its implementations behind it.**
  `IApiPlatformStateBroker` / `IApiPlatformTokenBroker` each get one node at
  the broker column, with the memory and session implementations to their
  right, because which one is live is a registration choice rather than a
  call.

## The data files

All data is declarative YAML — no code runs to produce the model, and
[index.html](./index.html) is a pure renderer (it holds both views,
`buildSingleCopyInstances` / `layoutBands` and `buildDuplicatedInstances` /
`layoutTrees`, dispatched on `state.view`, and should rarely need changes).

**`graph.yml`** is the manifest:

- `projects` — id, name, kind (`internal` / `library` / `external`) and the
  `file` holding that project's components. List order controls the
  single-copy view's band order.
- `roots` — component ids in layout order for the per-consumer view. A
  component flagged `shared` **must** appear here, or its inbound edges are
  silently dropped.
- `events` / `eventBroker` — empty / `null` today; ready for an event bus.

**`projects/<name>.yml`** declares one project's components:

```yaml
- id: CIS.Foundation
  name: CareIdentityService
  layer: foundation       # exposer|client|processing|orchestration|
                          # foundation|broker|external
  col: 4                  # layout column — map documented in graph.yml
  shared: true            # optional: consumers link to ONE copy
  utility: true           # optional: hidden behind the header toggle
  deriveMethods: true     # optional: rows derived from inbound edges
                          # (externals — rows can never drift from arrows)
  description: "..."
  methods: [...]
  calls:                  # outbound flows, one per call
    - from: <method or null>   # null = header-level link
      to: <component id>
      method: <method or null>
  publishes:              # for a future event bus
    - method: M
      event: E
  subscribes:
    - event: E
      handler: H
```

Strings containing anything beyond letters, digits, spaces, `_.-/()` are
double-quoted JSON strings — the renderer parses a deliberately small YAML
subset, so stick to the shapes above (single-line scalars, no anchors, no
multi-line blocks).

## Updating the graph

The data is a scanned snapshot of the source, not a build artifact — refresh
it whenever clients, services, brokers or cross-project wiring change by
running the `/update-dependency-graph` skill in Claude Code (defined in
`.claude/skills/update-dependency-graph/SKILL.md`). It re-scans the solution,
diffs against the current data files, updates them, and re-verifies the
rendered graph. Small changes (a new method, one new call) are comfortable
hand-edits in the project file that owns the calling component.
