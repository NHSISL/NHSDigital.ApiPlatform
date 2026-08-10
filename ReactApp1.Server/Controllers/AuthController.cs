// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHS.Digital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHS.Digital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions;
using ReactApp1.Server.Data;
using ReactApp1.Server.Models;

namespace ReactApp1.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IApiPlatformClient apiPlatformClient;
    private readonly ILogger<AuthController> logger;
    private readonly ApplicationDbContext context;

    public AuthController(
        IApiPlatformClient apiPlatformClient,
        ILogger<AuthController> logger,
        ApplicationDbContext context)
    {
        this.apiPlatformClient = apiPlatformClient;
        this.logger = logger;
        this.context = context;
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login(CancellationToken cancellationToken)
    {
        string url = await this.apiPlatformClient
            .CareIdentityServiceClient
            .BuildLoginUrlAsync(cancellationToken);

        this.logger.LogInformation("Initiating CIS2 authentication.");

        return Redirect(url);
    }

    [Authorize]
    [HttpGet("session")]
    public async Task<IActionResult> Session(CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated is not true)
        {
            return Unauthorized();
        }

        // Ensure an access token exists for the current session.
        string accessToken = await this.apiPlatformClient
            .CareIdentityServiceClient
            .GetAccessTokenAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized();
        }

        return Ok(new
        {
            sub = User.FindFirstValue(ClaimTypes.NameIdentifier),
            upn = User.FindFirstValue(ClaimTypes.Upn)
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await this.apiPlatformClient
            .CareIdentityServiceClient
            .LogoutAsync(cancellationToken);

        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync("bff-cookie");

        return Redirect(@"\");
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(
        [FromQuery] string code,
        [FromQuery] string state,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest("Authorization code is missing.");
        }

        try
        {
            var userInfo = await this.apiPlatformClient.CareIdentityServiceClient
                .GetUserInfoAsync(code, state, cancellationToken);

            string userInfoJson = JsonSerializer.Serialize(userInfo);

            User? user = await this.context.Users.FirstOrDefaultAsync(
                u => u.NhsIdUserUid == userInfo.NhsIdUserUid,
                cancellationToken);

            if (user is null)
            {
                user = new User
                {
                    NhsIdUserUid = userInfo.NhsIdUserUid,
                    Name = userInfo.Name,
                    Sub = userInfo.Sub,
                    RawUserInfo = userInfoJson,
                    LastLoginAt = DateTime.UtcNow,
                    IsAuthorised = false
                };

                this.context.Users.Add(user);
            }
            else
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.RawUserInfo = userInfoJson;
            }

            await this.context.SaveChangesAsync(cancellationToken);

            if (user.IsAuthorised is false)
            {
                await this.apiPlatformClient
                    .CareIdentityServiceClient
                    .LogoutAsync(cancellationToken);

                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync("bff-cookie");

                return Redirect("/unauthorised");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Name),
                new Claim(ClaimTypes.Name, userInfo.Name),
                new Claim(ClaimTypes.Upn, userInfo.NhsIdUserUid),
            };

            var identity = new ClaimsIdentity(claims, "OAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("bff-cookie", principal);

            return Redirect("/");
        }
        catch (CareIdentityServiceClientValidationException ex)
        {
            this.logger.LogWarning(ex, "OAuth callback validation failed.");
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        catch (CareIdentityServiceClientDependencyException ex)
        {
            this.logger.LogError(ex, "OAuth callback dependency failure.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                ex.InnerException?.Message ?? "Authentication service unavailable.");
        }
        catch (CareIdentityServiceClientServiceException ex)
        {
            this.logger.LogError(ex, "OAuth callback service failure.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ex.InnerException?.Message ?? "Authentication failed.");
        }
    }
}
