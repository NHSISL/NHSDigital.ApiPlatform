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
    /// Tests that require a live NHS API Platform conversation are marked with an explicit
    /// [Fact(Skip = "...")] rather than being silently skipped on missing configuration, so that a
    /// run without credentials reports them as skipped instead of passing vacuously.
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
    }
}
