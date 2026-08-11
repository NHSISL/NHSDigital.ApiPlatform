// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.Extensions.Configuration;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration
{
    /// <summary>
    /// Builds the API Platform configuration used by the integration tests.
    ///
    /// Endpoints come from appsettings.json. Credentials are deliberately left blank there and must
    /// be supplied out of band — either through appsettings.Development.json (git ignored) or through
    /// environment variables, for example:
    ///
    ///     ApiPlatform__CareIdentity__ClientId
    ///     ApiPlatform__CareIdentity__ClientSecret
    ///
    /// Tests that require a live NHS API Platform conversation check
    /// <see cref="HasCredentials"/> and are skipped when credentials are absent.
    /// </summary>
    internal static class ConfigurationProvider
    {
        internal static ApiPlatformConfigurations GetApiPlatformConfigurations()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            return configuration
                .GetSection("ApiPlatform")
                .Get<ApiPlatformConfigurations>() ?? new ApiPlatformConfigurations();
        }

        internal static bool HasCredentials()
        {
            ApiPlatformConfigurations configurations = GetApiPlatformConfigurations();

            return string.IsNullOrWhiteSpace(configurations.CareIdentity.ClientId) is false &&
                string.IsNullOrWhiteSpace(configurations.CareIdentity.ClientSecret) is false;
        }
    }
}
