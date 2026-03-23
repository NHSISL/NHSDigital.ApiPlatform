# NHSDigital.ApiPlatform.Sdk.AspNetCore

## Overview

`NHSDigital.ApiPlatform.Sdk.AspNetCore` is the ASP.NET Core adapter
for:

`NHSDigital.ApiPlatform.Sdk`

It provides:

-   Session-based token/state storage
-   Cookie-based token/state storage (BFF-style)
-   DI registration helpers
-   Seamless integration into ASP.NET Core applications

------------------------------------------------------------------------

## Installation

``` bash
dotnet add package NHSDigital.ApiPlatform.Sdk
dotnet add package NHSDigital.ApiPlatform.Sdk.AspNetCore
```

------------------------------------------------------------------------

## Configuration (appsettings.json)

``` json
{
  "ApiPlatform": {
    "CareIdentity": {
      "ClientId": "...",
      "ClientSecret": "...",
      "RedirectUri": "...",
      "AuthEndpoint": "...",
      "TokenEndpoint": "...",
      "UserInfoEndpoint": "...",
      "AcrValues": "aal3"
    },
    "PersonalDemographicsService": {
      "BaseUrl": "..."
    }
  }
}
```

------------------------------------------------------------------------

## Registration

### Session Mode (Recommended to Start)

``` csharp
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddApiPlatformSdkAspNetCore(
    builder.Configuration.GetSection("ApiPlatform")
        .Get<ApiPlatformConfigurations>(),
    storageMode: ApiPlatformAspNetStorageMode.Session);

app.UseSession();
```

### Cookie Mode (BFF-Style)

``` csharp
builder.Services.AddApiPlatformSdkAspNetCore(
    builder.Configuration.GetSection("ApiPlatform")
        .Get<ApiPlatformConfigurations>(),
    storageMode: ApiPlatformAspNetStorageMode.Cookies);
```

Cookies are: - HttpOnly - Secure (when HTTPS) - SameSite=Lax

------------------------------------------------------------------------

## Example Auth Controller

``` csharp
[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IApiPlatformClient api;

    public AuthController(IApiPlatformClient api) => this.api = api;

    [HttpGet("login")]
    public IActionResult Login()
    {
        string url = this.api.CareIdentityServices.Login();
        return Redirect(url);
    }

    [HttpGet("callback")]
    public IActionResult Callback(string code, string state)
    {
        this.api.CareIdentityServices.Callback(code, state);
        return Redirect("/");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        this.api.CareIdentityServices.Logout();
        return Redirect("/");
    }
}
```

------------------------------------------------------------------------

## Example PDS Controller

``` csharp
[ApiController]
[Route("pds")]
public sealed class PdsController : ControllerBase
{
    private readonly IApiPlatformClient api;

    public PdsController(IApiPlatformClient api) => this.api = api;

    [HttpGet("patients")]
    public async Task<IActionResult> Search(string family)
    {
        string result = await this.api
            .PersonalDemographicsServices
            .SearchPatientsAsync(family);

        return Content(result, "application/fhir+json");
    }
}
```

------------------------------------------------------------------------

## Refresh Token Renewal

All calls automatically use:

``` csharp
GetAccessToken()
```

If expired, the SDK:

1.  Uses refresh token
2.  Calls token endpoint
3.  Stores new tokens
4.  Continues execution

No extra developer code required.

------------------------------------------------------------------------

## Requirements

### Session Mode

-   `AddSession()`
-   `UseSession()`

### Cookie Mode

-   HTTPS recommended for production

------------------------------------------------------------------------

© North East London ICB
