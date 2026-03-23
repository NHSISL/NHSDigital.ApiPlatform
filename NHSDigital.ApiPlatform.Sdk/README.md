# NHSDigital.ApiPlatform.Sdk (Core)

## Overview

`NHSDigital.ApiPlatform.Sdk` is a host-agnostic .NET SDK that wraps the NHS Digital API Platform:

Currently support includes:
-   CIS2 Authentication (Authorization Code Flow)
-   PDS FHIR R4 client (example implementation)

(Future extensibility for additional NHS Digital APIs is planned.)

This package does **not** depend on ASP.NET Core.
You must provide implementations of token and state storage interfaces, 
allowing the SDK to work in any .NET host environment.

---

## Installation

```bash
dotnet add package NHSDigital.ApiPlatform.Sdk
```

---

## Configuration

The SDK is configured via `ApiPlatformConfigurations`:

```csharp
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;

var config = new ApiPlatformConfigurations
{
    CareIdentity = new CareIdentityConfigurations
    {
        ClientId = "...",
        ClientSecret = "...",
        RedirectUri = "...",
        AuthEndpoint = "...",
        TokenEndpoint = "...",
        UserInfoEndpoint = "...",
        AcrValues = "aal3" // optional
    },
    PersonalDemographicsService = new PersonalDemographicsServiceConfigurations
    {
        BaseUrl = "https://.../personal-demographics/FHIR/R4"
    }
};
```

---

## Storage Abstractions

The SDK relies on two storage abstractions:

- `IApiPlatformStateBroker` (CSRF state for the login flow)
- `IApiPlatformTokenBroker` (access/refresh tokens and expiry timestamps)

### Default Implementations (In-Memory)

The Core SDK includes optional in-memory implementations:

- `MemoryApiPlatformStateBroker`
- `MemoryApiPlatformTokenBroker`

These are suitable for:

- Development
- Prototypes
- Console applications / single-user processes

For production web applications, prefer a host-appropriate implementation (e.g., session, distributed cache, or database-backed storage). The ASP.NET Core package provides web-specific implementations.

---

## 🚀 Quick Start - Registration (DI)

Register the core services:

```csharp
using NHSDigital.ApiPlatform.Sdk;

services.AddApiPlatformSdkCore(config);
```

If you are running outside ASP.NET Core and want the in-memory defaults:

```csharp
services.AddApiPlatformSdkCore(config);
services.AddApiPlatformSdkInMemoryStorage();
```

### Production storage (non in-memory)
If you do not use the in-memory defaults, register your own implementations of:

-  IApiPlatformStateBroker
-  IApiPlatformTokenBroker

Example: register custom production-ready brokers (database, distributed cache, key vault, etc.):

```csharp
using NHSDigital.ApiPlatform.Sdk;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;

// Your implementations (examples)
services.AddSingleton<IApiPlatformStateBroker, MyProductionApiPlatformStateBroker>();
services.AddSingleton<IApiPlatformTokenBroker, MyProductionApiPlatformTokenBroker>();

services.AddApiPlatformSdkCore(config);
```

> ASP.NET Core applications should use the `NHSDigital.ApiPlatform.Sdk.AspNetCore` package to register web-specific storage.  [NHSDigital.ApiPlatform.Sdk.AspNetCore README](../NHSDigital.ApiPlatform.Sdk.AspNetCore/README.md)

---

## 🚀 Quick Start (No DI)

For simple hosts, you can instantiate the root client directly:

```csharp
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;

IApiPlatformClient apiPlatformClient = new ApiPlatformClient(config);
```

This constructor wires up the SDK internally and uses in-memory storage defaults.

### Using Custom Storage (No DI)

If you want to provide production-ready storage implementations 
(for example, database-backed or distributed cache), use the static Create method:

```csharp
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;

IApiPlatformStateBroker stateBroker = new MyProductionStateBroker();
IApiPlatformTokenBroker tokenBroker = new MyProductionTokenBroker();

IApiPlatformClient apiPlatformClient =
    ApiPlatformClient.Create(config, stateBroker, tokenBroker);
```
    ApiPlatformClient.Create(config, stateBroker, tokenBroker);

If either broker is omitted (or passed as null), 
the SDK will automatically fall back to the built-in in-memory implementations.

> The Create method is intended for non-DI scenarios.
ASP.NET Core applications should use the DI registration approach instead.  [NHSDigital.ApiPlatform.Sdk.AspNetCore README](../NHSDigital.ApiPlatform.Sdk.AspNetCore/README.md)
---

## Using the SDK

### Start Login

```csharp
string loginUrl = await apiPlatformClient
    .CareIdentityServiceClient
    .BuildLoginUrlAsync(cancellationToken);

return Redirect(loginUrl);
```

### Handle Callback and Retrieve User Info

Use the processing-based convenience method which completes the callback flow and returns user information:

```csharp
var userInfo = await apiPlatformClient
    .CareIdentityServiceClient
    .GetUserInfoAsync(code, state, cancellationToken);
```

This call:

1. Validates `code` and `state`
2. Exchanges the authorization code for tokens
3. Stores access and refresh tokens
4. Retrieves user information

### Retrieve an Access Token (Auto Refresh Enabled)

```csharp
string accessToken = await apiPlatformClient
    .CareIdentityServiceClient
    .GetAccessTokenAsync(cancellationToken);
```

If the access token is expired or expiring within the next 60 seconds, the SDK will refresh it using the refresh token, store the new tokens, and return the refreshed access token.

### Search Patients

```csharp
string responseJson = await apiPlatformClient
    .PersonalDemographicsServiceClient
    .SearchPatientsAsync(
        family: "Smith",
        given: new[] { "John" },
        gender: "male",
        birthdate: new DateOnly(1980, 1, 1),
        cancellationToken: cancellationToken);
```

---

© North East London ICB
