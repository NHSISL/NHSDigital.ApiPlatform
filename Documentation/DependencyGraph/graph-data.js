/* =====================================================================
   NHS Digital API Platform solution dependency data — consumed by
   index.html (both the single-copy and the per-consumer view).

   Hand-maintained model of the solution's components and flows,
   generated from the actual source (2026-08-11).

   Shape:
     projects:   { id, name, kind: internal|library|external }
     components: { id, name, project, layer, col, methods[], utility?,
                   shared?, description? }
        - col: layout column (left → right)
        - utility: hidden unless the "utility brokers" toggle is on
        - shared: consumers link to ONE copy (library/external exposers)
          instead of getting a duplicated copy each
     events:     { id, publish, subscribe }  (row labels on an event broker)
     edges:      direct    { kind:"direct", from:[comp,method|null],
                             to:[comp,method|null] }
                 publish   { kind:"publish", from:[comp,method], event }
                 subscribe { kind:"subscribe", event, to:[comp,handler] }
     roots:      component ids that start a tree (layout order)

   NOTE: this solution has no event bus — `events` is empty and every edge
   is a direct call (blue). The publish/subscribe machinery is left in the
   renderer so an event broker can be modelled later without touching
   index.html.
   ===================================================================== */

(function () {
  const projects = [
    { id: "sdk", name: "NHSDigital.ApiPlatform.Sdk", kind: "internal" },
    { id: "sdk-aspnetcore", name: "NHSDigital.ApiPlatform.Sdk.AspNetCore", kind: "internal" },
    { id: "infrastructure", name: "NHSDigital.ApiPlatform.Infrastructure", kind: "internal" },
    { id: "ext-http", name: "Microsoft.Extensions.Http", kind: "external" },
    { id: "ext-aspnetcore", name: "ASP.NET Core", kind: "external" },
    { id: "ext-bcl", name: ".NET base class library", kind: "external" },
    { id: "ext-adotnet", name: "ADotNet", kind: "external" },
    { id: "ext-logging", name: "Microsoft.Extensions.Logging", kind: "external" },
    { id: "ext-nhs", name: "NHS Digital API Platform (remote)", kind: "external" },
  ];

  const components = [];
  const events = [];
  const edges = [];
  const roots = [];

  const C = (comp) => { components.push(comp); return comp.id; };
  const D = (from, to) => edges.push({ kind: "direct", from, to });
  const P = (comp, method, event) => edges.push({ kind: "publish", from: [comp, method], event });
  const S = (event, comp, handler) => edges.push({ kind: "subscribe", event, to: [comp, handler] });

  /* ==================================================================
     Columns:
     0 SDK entry point        1 clients
     2 processings            3 orchestrations
     4 foundations            5 SDK brokers
     6 in-memory broker implementations (Sdk)
     7 session broker implementations (Sdk.AspNetCore)
     8 far externals
     ================================================================== */

  /* ==================================================================
     External surfaces (shared, single copy). Method rows are derived
     from the declared edges at the bottom of this file, so the rows and
     the arrows can never drift apart.
     ================================================================== */
  C({ id: "EXT.HttpClientFactory", name: "IHttpClientFactory / HttpClient", project: "ext-http", layer: "external", col: 8, shared: true, methods: [],
      description: "The named \"NhsApiPlatform\" client registered by AddApiPlatformSdkCore. HttpBroker is the only component that touches it." });
  C({ id: "EXT.Session", name: "ISession / IHttpContextAccessor", project: "ext-aspnetcore", layer: "external", col: 8, shared: true, methods: [],
      description: "ASP.NET Core session state. The Sdk.AspNetCore brokers throw when there is no HttpContext or session — the host must have called UseSession()." });
  C({ id: "EXT.Bcl", name: "System.Security.Cryptography / Text.Json", project: "ext-bcl", layer: "external", col: 8, shared: true, methods: [],
      description: "RandomNumberGenerator for the CSRF state, System.Text.Json (Web defaults) for payloads, Guid.NewGuid for the PDS X-Request-ID, DateTimeOffset.UtcNow for token expiry." });
  C({ id: "EXT.Logging", name: "Microsoft.Extensions.Logging", project: "ext-logging", layer: "external", col: 8,
      shared: true, utility: true, methods: [],
      description: "AddApiPlatformSdkCore calls AddLogging(), so a host that configures no providers still resolves an ILoggerFactory and the SDK's error logging goes nowhere rather than failing." });
  C({ id: "EXT.Cis2", name: "NHS CIS2 (Care Identity Service)", project: "ext-nhs", layer: "external", col: 8, shared: true, methods: [],
      description: "OAuth2 authorization-code flow without PKCE — CIS2 does not support it. Auth, token and userinfo endpoints come from CareIdentityConfigurations." });
  C({ id: "EXT.Pds", name: "NHS Personal Demographics Service", project: "ext-nhs", layer: "external", col: 8, shared: true, methods: [],
      description: "FHIR Patient search / retrieve. Requests carry a bearer token, a per-request X-Request-ID and an application/fhir+json Accept header." });
  C({ id: "EXT.ADotNet", name: "ADotNetClient", project: "ext-adotnet", layer: "external", col: 8, shared: true, methods: [],
      description: "Serialises the GithubPipeline object graph to YAML. ADotNet 4.1.0." });

  /* ==================================================================
     NHSDigital.ApiPlatform.Sdk — the public entry point.
     ================================================================== */
  C({ id: "ApiPlatformClient", name: "ApiPlatformClient", project: "sdk", layer: "exposer", col: 0,
      methods: ["Create", "CareIdentityServiceClient", "PersonalDemographicsServiceClient"],
      description: "The SDK's front door, usable two ways: resolved from DI (AddApiPlatformSdkCore), or built standalone via the static Create / the configurations-only constructor, which spins up its own ServiceCollection and falls back to the in-memory storage brokers. Exposes the two sub-clients as properties." });
  D(["ApiPlatformClient", "CareIdentityServiceClient"], ["CIS.Client", null]);
  D(["ApiPlatformClient", "PersonalDemographicsServiceClient"], ["PDS.Client", null]);

  C({ id: "ApiPlatformClientFacade", name: "ApiPlatformClientFacade", project: "sdk", layer: "exposer", col: 0,
      methods: ["CareIdentityServiceClient", "PersonalDemographicsServiceClient"],
      description: "DEAD CODE at the last scan: an internal IApiPlatformClient holding the same two sub-clients, but nothing constructs or registers it — AddApiPlatformSdkCore registers a hand-built ApiPlatformClient instead. It has no inbound flows on this graph." });

  /* ==================================================================
     Clients — the per-API surface each consumer actually calls.
     ================================================================== */
  C({ id: "CIS.Client", name: "CareIdentityServiceClient", project: "sdk", layer: "client", col: 1,
      methods: ["BuildLoginUrlAsync", "LogoutAsync", "GetAccessTokenAsync", "GetUserInfoAsync"],
      description: "Straight passthrough to the processing service — no logic of its own." });
  for (const m of ["BuildLoginUrlAsync", "LogoutAsync", "GetAccessTokenAsync", "GetUserInfoAsync"])
    D(["CIS.Client", m], ["CIS.Processing", m]);

  C({ id: "PDS.Client", name: "PersonalDemographicsServiceClient", project: "sdk", layer: "client", col: 1,
      methods: ["SearchPatientsAsync"],
      description: "Straight passthrough to the PDS orchestration." });
  D(["PDS.Client", "SearchPatientsAsync"], ["PDS.Orchestration", "SearchPatientsAsync"]);

  /* ==================================================================
     Processing — the CIS2 login dance, sequenced.
     ================================================================== */
  C({ id: "CIS.Processing", name: "CareIdentityServiceProcessingService", project: "sdk", layer: "processing", col: 2,
      methods: ["BuildLoginUrlAsync", "LogoutAsync", "GetAccessTokenAsync", "GetUserInfoAsync"],
      description: "Thin over the foundation service except for GetUserInfoAsync, which is the whole OAuth callback in one call: complete the callback (state check + code exchange), read the freshly stored access token, then fetch the profile." });
  D(["CIS.Processing", "BuildLoginUrlAsync"], ["CIS.Foundation", "BuildLoginUrlAsync"]);
  D(["CIS.Processing", "LogoutAsync"], ["CIS.Foundation", "LogoutAsync"]);
  D(["CIS.Processing", "GetAccessTokenAsync"], ["CIS.Foundation", "GetAccessTokenAsync"]);
  D(["CIS.Processing", "GetUserInfoAsync"], ["CIS.Foundation", "CallbackAsync"]);
  D(["CIS.Processing", "GetUserInfoAsync"], ["CIS.Foundation", "GetAccessTokenAsync"]);
  D(["CIS.Processing", "GetUserInfoAsync"], ["CIS.Foundation", "GetUserInfoAsync"]);

  /* ==================================================================
     Orchestration — the only place the two APIs meet.
     ================================================================== */
  C({ id: "PDS.Orchestration", name: "PdsOrchestrationService", project: "sdk", layer: "orchestration", col: 3,
      methods: ["SearchPatientsAsync"],
      description: "Validates the search criteria, gets a CIS2 access token, refuses the call with UnauthorizedPdsOrchestrationException when it comes back empty, then hands it to PdsService." });
  D(["PDS.Orchestration", "SearchPatientsAsync"], ["CIS.Foundation", "GetAccessTokenAsync"]);
  D(["PDS.Orchestration", "SearchPatientsAsync"], ["PDS.Foundation", "SearchPatientsAsync"]);

  /* ==================================================================
     Foundations.
     ================================================================== */
  C({ id: "CIS.Foundation", name: "CareIdentityService", project: "sdk", layer: "foundation", col: 4,
      methods: ["BuildLoginUrlAsync", "LogoutAsync", "CallbackAsync", "GetAccessTokenAsync", "GetUserInfoAsync"],
      description: "The CIS2 OAuth2 implementation. BuildLoginUrl mints a CSRF state and stashes it; Callback compares the returned state, clears it, exchanges the code and stores both tokens with computed expiries; GetAccessToken serves the stored token while it has more than 60 seconds left, otherwise silently refreshes off the refresh token and returns empty when that has expired too. The private ExchangeCodeForTokenAsync / ExchangeRefreshTokenForTokenAsync helpers are where the token endpoint is actually hit — their calls are attributed to CallbackAsync and GetAccessTokenAsync respectively." });

  const cisB = (from, to) => D(["CIS.Foundation", from], to);
  cisB("BuildLoginUrlAsync", ["CryptoBroker", "CreateUrlSafeState"]);
  cisB("BuildLoginUrlAsync", ["StateBroker", "StoreCsrfStateAsync"]);
  cisB("LogoutAsync", ["StateBroker", "ClearCsrfStateAsync"]);
  cisB("LogoutAsync", ["TokenBroker", "ClearAccessTokenAsync"]);
  cisB("LogoutAsync", ["TokenBroker", "ClearRefreshTokenAsync"]);
  // Callback: state check, then ExchangeCodeForToken + GetUserInfo, then store
  cisB("CallbackAsync", ["StateBroker", "GetCsrfStateAsync"]);
  cisB("CallbackAsync", ["StateBroker", "ClearCsrfStateAsync"]);
  cisB("CallbackAsync", ["HttpBroker", "PostFormAsync"]);
  cisB("CallbackAsync", ["HttpBroker", "GetAsync"]);
  cisB("CallbackAsync", ["JsonBroker", "Deserialize"]);
  cisB("CallbackAsync", ["DateTimeBroker", "GetCurrentDateTimeOffset"]);
  cisB("CallbackAsync", ["TokenBroker", "StoreAccessTokenAsync"]);
  cisB("CallbackAsync", ["TokenBroker", "StoreRefreshTokenAsync"]);
  // GetAccessToken: read, and on the refresh path ExchangeRefreshTokenForToken + store
  cisB("GetAccessTokenAsync", ["TokenBroker", "GetAccessTokenAsync"]);
  cisB("GetAccessTokenAsync", ["TokenBroker", "GetRefreshTokenAsync"]);
  cisB("GetAccessTokenAsync", ["DateTimeBroker", "GetCurrentDateTimeOffset"]);
  cisB("GetAccessTokenAsync", ["HttpBroker", "PostFormAsync"]);
  cisB("GetAccessTokenAsync", ["JsonBroker", "Deserialize"]);
  cisB("GetAccessTokenAsync", ["TokenBroker", "StoreAccessTokenAsync"]);
  cisB("GetAccessTokenAsync", ["TokenBroker", "StoreRefreshTokenAsync"]);
  cisB("GetUserInfoAsync", ["HttpBroker", "GetAsync"]);
  cisB("GetUserInfoAsync", ["JsonBroker", "Deserialize"]);

  C({ id: "PDS.Foundation", name: "PdsService", project: "sdk", layer: "foundation", col: 4,
      methods: ["SearchPatientsAsync"],
      description: "Builds the PDS URL — /Patient/{nhsNumber} when an NHS number is supplied, otherwise a demographics query built from surname plus any of given / gender / birthdate / postcode — and issues the request with a bearer token, a fresh X-Request-ID and an application/fhir+json Accept header. Returns the raw FHIR JSON; nothing in the SDK deserialises it." });
  D(["PDS.Foundation", "SearchPatientsAsync"], ["HttpBroker", "GetAsync"]);
  D(["PDS.Foundation", "SearchPatientsAsync"], ["IdentifierBroker", "GetNewGuid"]);

  /* ==================================================================
     Brokers.
     ================================================================== */
  C({ id: "HttpBroker", name: "HttpBroker", project: "sdk", layer: "broker", col: 5,
      methods: ["PostFormAsync", "GetAsync"],
      description: "Resolves the named \"NhsApiPlatform\" HttpClient per call. GetAsync takes a configureRequest callback so callers can add their own headers without the broker knowing about them." });
  D(["HttpBroker", "PostFormAsync"], ["EXT.HttpClientFactory", "CreateClient(\"NhsApiPlatform\")"]);
  D(["HttpBroker", "PostFormAsync"], ["EXT.HttpClientFactory", "HttpClient.PostAsync"]);
  D(["HttpBroker", "GetAsync"], ["EXT.HttpClientFactory", "CreateClient(\"NhsApiPlatform\")"]);
  D(["HttpBroker", "GetAsync"], ["EXT.HttpClientFactory", "HttpClient.SendAsync"]);
  D(["HttpBroker", "PostFormAsync"], ["EXT.Cis2", "POST token endpoint"]);
  D(["HttpBroker", "GetAsync"], ["EXT.Cis2", "GET userinfo endpoint"]);
  D(["HttpBroker", "GetAsync"], ["EXT.Pds", "GET /Patient"]);

  C({ id: "CryptoBroker", name: "CryptoBroker", project: "sdk", layer: "broker", col: 5,
      methods: ["CreateUrlSafeState"],
      description: "32 random bytes, base64 then made URL-safe (trim =, + to -, / to _). This is the CSRF state for the CIS2 round trip." });
  D(["CryptoBroker", "CreateUrlSafeState"], ["EXT.Bcl", "RandomNumberGenerator.Fill"]);

  C({ id: "JsonBroker", name: "JsonBroker", project: "sdk", layer: "broker", col: 5,
      methods: ["Deserialize", "Serialize"],
      description: "System.Text.Json with JsonSerializerDefaults.Web. Serialize is part of the surface but nothing in the SDK calls it today." });
  D(["JsonBroker", "Deserialize"], ["EXT.Bcl", "JsonSerializer.Deserialize"]);
  D(["JsonBroker", "Serialize"], ["EXT.Bcl", "JsonSerializer.Serialize"]);

  C({ id: "DateTimeBroker", name: "DateTimeBroker", project: "sdk", layer: "broker", col: 5, utility: true,
      methods: ["GetCurrentDateTimeOffset"] });
  D(["DateTimeBroker", "GetCurrentDateTimeOffset"], ["EXT.Bcl", "DateTimeOffset.UtcNow"]);
  C({ id: "IdentifierBroker", name: "IdentifierBroker", project: "sdk", layer: "broker", col: 5, utility: true,
      methods: ["GetNewGuid"] });
  D(["IdentifierBroker", "GetNewGuid"], ["EXT.Bcl", "Guid.NewGuid"]);

  C({ id: "LoggingBroker", name: "LoggingBroker", project: "sdk", layer: "broker", col: 5, utility: true,
      methods: ["LogErrorAsync", "LogCriticalAsync"],
      description: "Wraps ILogger<LoggingBroker>. Every service takes it, but it is only ever reached from the CreateAndLog* exception factories — which this graph deliberately does not draw — so it has no inbound flows here. Registered with TryAddSingleton so a host can substitute its own." });
  D(["LoggingBroker", "LogErrorAsync"], ["EXT.Logging", "ILogger.LogError"]);
  D(["LoggingBroker", "LogCriticalAsync"], ["EXT.Logging", "ILogger.LogCritical"]);

  /* -- the two swappable storage brokers -------------------------------
     Both interfaces have an in-memory implementation shipped in the Sdk
     and a session-backed one in Sdk.AspNetCore. Which one you get is a
     registration choice, so the interface is drawn once and both
     implementations hang off it.
     ------------------------------------------------------------------ */
  C({ id: "StateBroker", name: "IApiPlatformStateBroker", project: "sdk", layer: "broker", col: 5,
      methods: ["StoreCsrfStateAsync", "GetCsrfStateAsync", "ClearCsrfStateAsync"],
      description: "Holds the CSRF state between the login redirect and the callback. AddApiPlatformSdkInMemoryStorage registers the in-memory copy with TryAdd, so a host that has already registered the session one keeps it." });
  C({ id: "TokenBroker", name: "IApiPlatformTokenBroker", project: "sdk", layer: "broker", col: 5,
      methods: ["StoreAccessTokenAsync", "GetAccessTokenAsync", "ClearAccessTokenAsync",
                "StoreRefreshTokenAsync", "GetRefreshTokenAsync", "ClearRefreshTokenAsync"],
      description: "Holds the access and refresh tokens with their expiry instants. Same TryAdd registration story as the state broker." });

  C({ id: "MemoryStateBroker", name: "MemoryApiPlatformStateBroker", project: "sdk", layer: "broker", col: 6,
      methods: ["StoreCsrfStateAsync", "GetCsrfStateAsync", "ClearCsrfStateAsync"],
      description: "A single lock-guarded field. Registered as a singleton, so it is process-wide — fine for a console app or a test, wrong for a multi-user web host." });
  C({ id: "MemoryTokenBroker", name: "MemoryApiPlatformTokenBroker", project: "sdk", layer: "broker", col: 6,
      methods: ["StoreAccessTokenAsync", "GetAccessTokenAsync", "ClearAccessTokenAsync",
                "StoreRefreshTokenAsync", "GetRefreshTokenAsync", "ClearRefreshTokenAsync"],
      description: "In-process token store, singleton. Same single-user caveat as the memory state broker." });
  C({ id: "SessionStateBroker", name: "SessionApiPlatformStateBroker", project: "sdk-aspnetcore", layer: "broker", col: 7,
      methods: ["StoreCsrfStateAsync", "GetCsrfStateAsync", "ClearCsrfStateAsync"],
      description: "Reads and writes ASP.NET Core session state via IHttpContextAccessor, scoped per request. Throws if there is no HttpContext or the session has not been enabled." });
  C({ id: "SessionTokenBroker", name: "SessionApiPlatformTokenBroker", project: "sdk-aspnetcore", layer: "broker", col: 7,
      methods: ["StoreAccessTokenAsync", "GetAccessTokenAsync", "ClearAccessTokenAsync",
                "StoreRefreshTokenAsync", "GetRefreshTokenAsync", "ClearRefreshTokenAsync"],
      description: "Session-backed tokens; expiries are stored as unix seconds under the keys in SessionApiPlatformStorageKeys." });

  for (const m of ["StoreCsrfStateAsync", "GetCsrfStateAsync", "ClearCsrfStateAsync"]) {
    D(["StateBroker", m], ["MemoryStateBroker", m]);
    D(["StateBroker", m], ["SessionStateBroker", m]);
    D(["SessionStateBroker", m], ["EXT.Session", "ISession"]);
  }
  for (const m of ["StoreAccessTokenAsync", "GetAccessTokenAsync", "ClearAccessTokenAsync",
                   "StoreRefreshTokenAsync", "GetRefreshTokenAsync", "ClearRefreshTokenAsync"]) {
    D(["TokenBroker", m], ["MemoryTokenBroker", m]);
    D(["TokenBroker", m], ["SessionTokenBroker", m]);
    D(["SessionTokenBroker", m], ["EXT.Session", "ISession"]);
  }
  D(["SessionStateBroker", "GetCsrfStateAsync"], ["EXT.Session", "IHttpContextAccessor.HttpContext"]);
  D(["SessionTokenBroker", "GetAccessTokenAsync"], ["EXT.Session", "IHttpContextAccessor.HttpContext"]);

  /* ==================================================================
     NHSDigital.ApiPlatform.Infrastructure — generates the CI workflows.
     ================================================================== */
  C({ id: "INF.Program", name: "Program", project: "infrastructure", layer: "exposer", col: 0,
      methods: ["Main"],
      description: "Console entry point. Running this project rewrites .github/workflows/build.yml and prLinter.yml — they are generated artifacts, not hand-edited files. (pages.yml is the exception: it is hand-authored, because the Pages actions are outside ADotNet 4.1.0's task model.)" });
  C({ id: "INF.ScriptGeneration", name: "ScriptGenerationService", project: "infrastructure", layer: "foundation", col: 4,
      methods: ["GenerateBuildScript", "GeneratePrLintScript"],
      description: "Builds a GithubPipeline object graph — build on push/PR to main against .NET 10, and the PR linter's label + issue-association jobs — and serialises it with ADotNet." });
  D(["INF.Program", "Main"], ["INF.ScriptGeneration", "GenerateBuildScript"]);
  D(["INF.Program", "Main"], ["INF.ScriptGeneration", "GeneratePrLintScript"]);
  D(["INF.ScriptGeneration", "GenerateBuildScript"], ["EXT.ADotNet", "SerializeAndWriteToFile"]);
  D(["INF.ScriptGeneration", "GeneratePrLintScript"], ["EXT.ADotNet", "SerializeAndWriteToFile"]);

  /* ==================================================================
     roots — tree order controls the vertical layout
     ================================================================== */
  roots.push(
    // NHSDigital.ApiPlatform.Sdk
    "ApiPlatformClient", "ApiPlatformClientFacade",
    "CIS.Client", "PDS.Client",
    "CIS.Processing", "PDS.Orchestration",
    "CIS.Foundation", "PDS.Foundation",
    "HttpBroker", "CryptoBroker", "JsonBroker", "DateTimeBroker", "IdentifierBroker", "LoggingBroker",
    "StateBroker", "TokenBroker", "MemoryStateBroker", "MemoryTokenBroker",
    // NHSDigital.ApiPlatform.Sdk.AspNetCore
    "SessionStateBroker", "SessionTokenBroker",
    // NHSDigital.ApiPlatform.Infrastructure
    "INF.Program", "INF.ScriptGeneration",
    // externals
    "EXT.HttpClientFactory", "EXT.Session", "EXT.Bcl", "EXT.Logging", "EXT.Cis2", "EXT.Pds", "EXT.ADotNet",
  );

  /* ------------------------------------------------------------------
     Externals show exactly the public surface this solution calls.
     Derive their method rows from the declared edges so the rows and
     the arrows can never drift apart.
     ------------------------------------------------------------------ */
  for (const extId of ["EXT.HttpClientFactory", "EXT.Session", "EXT.Bcl", "EXT.Logging", "EXT.Cis2", "EXT.Pds", "EXT.ADotNet"]) {
    const comp = components.find(c => c.id === extId);
    const called = [];
    for (const e of edges) {
      if (e.kind === "direct" && e.to[0] === extId && e.to[1] && !called.includes(e.to[1])) called.push(e.to[1]);
    }
    comp.methods = called.sort((a, b) => a.localeCompare(b));
  }

  window.APIPLATFORM_DATA = {
    projects,
    components,
    events,
    edges,
    roots,
    eventBrokerId: null,
  };
})();
