// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Microsoft.Extensions.Configuration;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using Tynamix.ObjectFiller;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        private readonly ApiPlatformConfigurations apiPlatformConfigurations;
        private readonly IApiPlatformClient apiPlatformClient;
        private readonly ICareIdentityServiceClient careIdentityServiceClient;

        public CareIdentityServiceClientTests()
        {
            this.apiPlatformConfigurations = ConfigurationProvider.GetApiPlatformConfigurations();
            this.apiPlatformClient = new ApiPlatformClient(this.apiPlatformConfigurations);
            this.careIdentityServiceClient = this.apiPlatformClient.CareIdentityServiceClient;
        }

        private static string ExtractQueryValue(string url, string key)
        {
            string query = new Uri(url).Query.TrimStart('?');

            foreach (string pair in query.Split('&'))
            {
                string[] parts = pair.Split('=');

                if (parts.Length == 2 && parts[0] == key)
                {
                    return parts[1];
                }
            }

            return string.Empty;
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 8, wordMaxLength: 12).GetValue();
    }
}
