using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NHS.Digital.ApiPlatform.Sdk;
using NHS.Digital.ApiPlatform.Sdk.AspNetCore;
using NHS.Digital.ApiPlatform.Sdk.Models.Configurations;
using ReactApp1.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("iDecide")));

builder.Services.AddHttpClient();

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("SessionCache");
    options.SchemaName = "dbo";
    options.TableName = "SessionCache";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax; // ✅ CRITICAL FIX: Changed from Strict
    options.Cookie.Name = ".IDecide.Session"; // ✅ Explicit naming
});

// SECURITY FIX: Store keys securely (NOT in c:\temp)
var keysPath = builder.Environment.IsProduction()
    ? Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys")
    : Path.Combine(Path.GetTempPath(), "IDecide-DataProtection-Dev");

Directory.CreateDirectory(keysPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("IDecide")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

builder.Services.AddAuthentication("bff-cookie")
    .AddCookie("bff-cookie", options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax; // ✅ Also fix for auth cookie
        options.Cookie.Name = "bff-cookie";
    });

// NHS Digital API Platform SDK (Core + AspNetCore/session storage)
ApiPlatformConfigurations apiPlatformConfigurations = new()
{
    CareIdentity = new CareIdentityConfigurations
    {
        ClientId = builder.Configuration["CIS:ClientId"] ?? string.Empty,
        ClientSecret = builder.Configuration["CIS:ClientSecret"] ?? string.Empty,
        RedirectUri = builder.Configuration["CIS:RedirectUri"] ?? string.Empty,
        AuthEndpoint = builder.Configuration["CIS:AuthEndpoint"] ?? string.Empty,
        TokenEndpoint = builder.Configuration["CIS:TokenEndpoint"] ?? string.Empty,
        UserInfoEndpoint = builder.Configuration["CIS:UserInfoEndpoint"] ?? string.Empty,
        AcrValues = builder.Configuration["CIS:AALLevel"]
    },
    PersonalDemographicsService = new PersonalDemographicsServiceConfigurations
    {
        BaseUrl = builder.Configuration["PDS:BaseUrl"]
            ?? "https://int.api.service.nhs.uk/personal-demographics/FHIR/R4"
    }
};

builder.Services.AddApiPlatformSdkCore(apiPlatformConfigurations);
builder.Services.AddApiPlatformSdkAspNetCore();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSession();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");
app.Run();
