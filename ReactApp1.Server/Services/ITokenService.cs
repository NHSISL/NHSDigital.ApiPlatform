namespace ReactApp1.Server.Services;

public interface ITokenService
{
    Task<string?> GetAccessTokenAsync(HttpContext httpContext);
}