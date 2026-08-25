---
name: update-dependency-graph
description: Re-scan the solution and regenerate the dependency graph data files (Documentation/DependencyGraph/graph.yml + projects/*.yml) so the interactive dependency graph matches the current source. Use when clients, services, brokers, storage implementations or cross-project wiring have changed, or when the user asks to refresh/rebuild the dependency graph.
version: 0.1.0
---

# Update Solution Dependency Graph

Regenerate the data files — `Documentation/DependencyGraph/graph.yml` and
`projects/*.yml` — from the current source. `index.html` is the renderer — do not change it unless a new concept
cannot be expressed in data (new edge kind, new layer). It carries BOTH views
behind `state.view`: `buildSingleCopyInstances` + `layoutBands` (the default)
and `buildDuplicatedInstances` + `layoutTrees`. Anything you change in one
builder usually needs the mirror change in the other.

## 1/ Load the current model

Read `Documentation/DependencyGraph/README.md`, then `graph.yml` and the
`projects/*.yml` files it lists. The data files are the previous scan's
snapshot; your job is a diff-and-update,
not a rewrite. Preserve its modelling rules:

- Per-consumer duplication is done by the renderer — declare each component
  ONCE; never hand-duplicate.
- `shared: true` on external surfaces. A `shared` component MUST also be in
  `roots` or its inbound edges are silently dropped.
- `utility: true` on the DateTime / Identifier brokers (hidden behind a
  toggle).
- Happy-path calls are drawn; exception-path (`TryCatch` / `CreateAndLog*`)
  logging is NOT. The `.Validations.cs` partials here are pure argument
  checks with no broker calls.
- Private helpers are attributed to the public method that reaches them. No
  component links to itself — a self-edge means you modelled a private helper
  as a row, which this graph does not do.
- A swappable interface is drawn once at the broker column with its
  implementations to the right (`IApiPlatformStateBroker` /
  `IApiPlatformTokenBroker` → memory + session), because which one is live is
  a registration choice rather than a call.
- This solution has no event bus — `events` is empty, `eventBrokerId` is
  `null`, and every edge is `kind: "direct"`. If an event broker ever lands,
  the renderer already supports `P(...)` / `S(...)` and automatic
  circular-flow detection; do not hand-colour anything.
- Column map (0–8) is documented in `graph.yml` — keep new components
  consistent with it.

## 2/ Re-scan the source

Read the interfaces for the public surface and the implementation `.cs` for
the per-method calls. A quick way to get per-method dependency calls out of a
C# tree is a small throwaway script that finds method declarations and then
the `this.<field>.<Method>` calls between one declaration and the next
(whitespace-normalise first — calls wrap across lines).

1. **`NHSDigital.ApiPlatform.Sdk`** — the whole SDK:
   - `Clients\*` — `ApiPlatformClient` (note its standalone `Create` path
     builds its own `ServiceCollection`), the facade, and the two per-API
     clients.
   - `Services\Processings\*`, `Services\Orchestrations\*`,
     `Services\Foundations\*` — dependencies and per-method calls.
   - `Brokers\*` — public surface plus the external member each one wraps
     (`IHttpClientFactory`, `System.Text.Json`, `RandomNumberGenerator`,
     `Guid`, `DateTimeOffset`).
   - `ServiceCollectionExtensions.cs` — the registration story, including
     which lifetimes and which `TryAdd` calls decide who wins.
2. **`NHSDigital.ApiPlatform.Sdk.AspNetCore`** — the session-backed state and
   token brokers and `AddApiPlatformSdkAspNetCore`.
3. **`NHSDigital.ApiPlatform.Infrastructure`** — `Program.Main` and
   `ScriptGenerationService`. Remember `.github/workflows/build.yml` and
   `prLinter.yml` are GENERATED from here; `pages.yml` is the one hand-authored
   workflow.
4. **Unused surface is a headline.** Check for packages referenced in a
   `.csproj` that no `.cs` file mentions, constructor dependencies that are
   never called, and public members with no callers — several exist today and
   they are recorded in the README's "Current truths".

## 3/ Update the data files

The YAML schema is documented in the README's "The data files" section —
components live in `projects/<project>.yml`, each with `methods` and its
outbound `calls` (`from: null` = header-level link); manifest-level lists
(`projects`, `roots`, `events`) live in `graph.yml`.

- A new component → add it to its project's file AND to `roots` in
  `graph.yml` (project order; `shared` components must be roots).
- Externals with `deriveMethods: true` get their rows derived from inbound
  edges at load time — never hand-list rows on them.
- Strings with characters beyond letters, digits, spaces and `_.-/()` must be
  double-quoted JSON strings; the renderer parses a small YAML subset
  (single-line scalars only, no anchors, no multi-line blocks).
- If an edit does not show up, load the page and watch for its "graph data
  did not load" panel — it prints the parse error.

## 4/ Verify in the browser

Serve the folder over HTTP — the page fetches graph.yml and the project
files, and browsers block those fetches from file:// pages:
instead of the graph:

```bash
python -m http.server 8731 --bind 127.0.0.1
```

Verify BOTH views — the header toggle, or `window.__graph.setView("single")` /
`window.__graph.setView("duplicated")` from `javascript_tool` (the renderer
exposes `window.__graph` = { state, setView, select, selectRow,
clearSelection, rebuild, fit, tracePath }). Confirm:

- No console errors; the header count is in the expected range (last scan:
  25 components · 79 flows single-copy; 100 nodes · 413 flows per consumer,
  27 · 84 and 113 · 441 with utility brokers on).
- No node-rect overlaps and no project-box overlaps — query `window.__graph.state.instances`
  and `.projBoxes` with `javascript_tool` and intersect pairwise, in each
  view, with the utility toggle both off and on.
- No dropped edges: every `shared` component appears in `roots`.
- Click one client, one foundation service and one method row: the side-panel
  flows in / out must match the scan.
- Selecting a header must light the component's whole fan-out (the same
  upstream + downstream slice a method row gets, seeded from every row), not
  just its first hop, and the selection must be outlined in amber. Clearing
  the selection must restore the graph exactly — snapshot every node's
  attributes before and after and compare.
- Switching view preserves the selection (by component id).

## 5/ Finish

Update the "Current truths" section and scan date in
`Documentation/DependencyGraph/README.md` (and the node/flow counts if they
moved), and summarize what changed since the previous snapshot — new
components, new flows, anything that became unreachable or newly consumed.
