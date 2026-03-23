![NHS Digital API Platform SDK](Resources/Images/NhsDigitalBanner.png)

# NHS Digital API Platform SDK

This repository provides a SDK for integrating with the
**NHS Digital API Platform**, including:

-   Care Identity Service (CIS2) Client 
    -   Authentication (Authorization Code Flow)
    -   Automatic refresh-token renewal
    
-   Personal Demographics Service (PDS) Client

(Future extensibility for additional NHS Digital APIs is planned.)

------------------------------------------------------------------------

# 📦 Packages

This solution is intentionally split into two focused packages:

- **`NHSDigital.ApiPlatform.Sdk` (Core)**
- **`NHSDigital.ApiPlatform.Sdk.AspNetCore` (Web Integration)**

## 🎯 Why Two Packages?

The SDK was deliberately designed to be **host-agnostic**.

The Core package contains:

- NHS API integration logic
- Authentication and token lifecycle management
- API client implementations
- No dependency on ASP.NET Core

It does **not** reference:

- `HttpContext`
- `IHttpContextAccessor`
- `ISession`
- ASP.NET Core middleware
- Web-specific abstractions

This design ensures the Core SDK can be used in:

- Console applications
- Background services
- Azure Functions
- Worker services
- Integration pipelines
- Custom web frameworks
- ASP.NET Core (via the integration package)

---

## 🌐 The ASP.NET Core Integration Package

`NHSDigital.ApiPlatform.Sdk.AspNetCore` provides:

- Session-based implementations of:
  - `IApiPlatformStateBroker`
  - `IApiPlatformTokenBroker`
- `IServiceCollection` extension methods
- ASP.NET Core-specific wiring
- Access to `HttpContext` and session safely

This package depends on ASP.NET Core abstractions, but the Core SDK does not.

---

## 🧠 Design Rationale

If the Core SDK directly referenced ASP.NET Core types such as:

- `IHttpContextAccessor`
- `HttpContext`
- `ISession`

then:

- Console applications could not use it.
- Worker services would carry unnecessary web dependencies.
- Unit testing would become more complex.
- The SDK would violate separation-of-concerns principles.

By splitting responsibilities:

| Concern | Package |
|---------|----------|
| Authentication logic | Core |
| Token refresh logic | Core |
| HTTP calls to NHS APIs | Core |
| Web session integration | AspNetCore |

This keeps the architecture:

- Clean
- Modular
- Testable
- Replaceable
- Environment-agnostic

------------------------------------------------------------------------

## 1️ NHSDigital.ApiPlatform.Sdk (Core)

Host-agnostic .NET SDK that:

This package can be used in:

-   Console applications
-   Background services
-   Azure Functions
-   Custom web frameworks
-   ASP.NET (with your own storage implementation)

👉 **Full documentation:**\
[NHSDigital.ApiPlatform.Sdk README](NHSDigital.ApiPlatform.Sdk/README.md)

------------------------------------------------------------------------

## 2️ NHSDigital.ApiPlatform.Sdk.AspNetCore

ASP.NET Core adapter for the core SDK.

Provides:

-   Session-based storage implementation
-   Cookie-based storage implementation (BFF-style)
-   ASP.NET DI registration helpers
-   Minimal setup for web applications

This is the recommended package for:

-   ASP.NET Core MVC
-   Web APIs
-   BFF architectures

👉 **Full documentation:**\
[NHSDigital.ApiPlatform.Sdk.AspNetCore README](NHSDigital.ApiPlatform.Sdk.AspNetCore/README.md)

------------------------------------------------------------------------

# 🏗 Architecture Overview

Internally, the SDK follows **The Standard** layering:

-   Brokers 
-   Foundations 
-   Validations
-   Orchestrations (workflow + auto-refresh logic)
-   Exposers (`IApiPlatformClient` surface)

This ensures:

-   Predictable structure
-   High testability
-   Clean separation of concerns
-   Future extensibility for additional NHS APIs

------------------------------------------------------------------------

# 🔐 Refresh Token Handling

The SDK automatically:

1.  Detects expired (or near-expiry) access tokens
2.  Uses the refresh token
3.  Requests new tokens from CIS2
4.  Stores updated tokens
5.  Continues execution seamlessly

No additional developer logic is required.

------------------------------------------------------------------------

# 🧪 Testing

The solution includes:

-   Unit test projects
-   Acceptance test scaffolding
-   Integration test scaffolding
-   Refresh-token renewal tests

------------------------------------------------------------------------

# 🚀 Getting Started

## ASP.NET Core Applications

1. Install the ASP.NET Core integration package:

    `dotnet add package NHSDigital.ApiPlatform.Sdk.AspNetCore`

   (This package automatically installs the Core SDK as a dependency.)

2. Configure `ApiPlatform` in `appsettings.json`.

3. Register the SDK:

   ```cs
   services.AddApiPlatformSdkCore(config);
   services.AddApiPlatformSdkAspNetCore();
   ```

4. Inject and use `IApiPlatformClient` via DI.
   
   Example: initiating CIS2 login from a controller:
    ```cs
    using Microsoft.AspNetCore.Mvc;
    using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;

    public class AuthController : Controller
    {
        private readonly IApiPlatformClient apiPlatformClient;

        public AuthController(IApiPlatformClient apiPlatformClient) =>
            this.apiPlatformClient = apiPlatformClient;

        [HttpGet("login")]
        public async Task<IActionResult> Login(CancellationToken cancellationToken)
        {
            string loginUrl = await this.apiPlatformClient
                .CareIdentityServiceClient
                .BuildLoginUrlAsync(cancellationToken);

            return Redirect(loginUrl);
        }
    }
    ```
    👉 **Full documentation:**\
    [NHSDigital.ApiPlatform.Sdk.AspNetCore README](NHSDigital.ApiPlatform.Sdk.AspNetCore/README.md)

------------------------------------------------------------------------

## Advanced / Non-ASP.NET Hosts

Install the Core package directly:

    `dotnet add package NHSDigital.ApiPlatform.Sdk`

You must implement:

- `IApiPlatformStateBroker`
- `IApiPlatformTokenBroker`

Then register:

    `services.AddApiPlatformSdkCore(config);`


👉 **Full documentation:**\
[NHSDigital.ApiPlatform.Sdk README](NHSDigital.ApiPlatform.Sdk/README.md)

------------------------------------------------------------------------

© North East London ICB
