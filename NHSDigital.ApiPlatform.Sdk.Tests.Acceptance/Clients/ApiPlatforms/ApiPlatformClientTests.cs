// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Tynamix.ObjectFiller;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Acceptance.Clients.ApiPlatforms
{
    [Collection(nameof(ApiPlatformClientTests))]
    public partial class ApiPlatformClientTests : IDisposable
    {
        private const string TokenPath = "/oauth2/token";
        private const string UserInfoPath = "/oauth2/userinfo";
        private const string AuthorizePath = "/oauth2/authorize";
        private const string FhirPath = "/personal-demographics/FHIR/R4";

        private readonly WireMockServer wireMockServer;
        private readonly ApiPlatformConfigurations apiPlatformConfigurations;
        private readonly IApiPlatformClient apiPlatformClient;
        private readonly ICareIdentityServiceClient careIdentityServiceClient;
        private readonly IPersonalDemographicsServiceClient personalDemographicsServiceClient;

        public ApiPlatformClientTests()
        {
            this.wireMockServer = WireMockServer.Start();
            string baseUrl = this.wireMockServer.Urls[0];

            this.apiPlatformConfigurations = new ApiPlatformConfigurations
            {
                CareIdentity = new CareIdentityConfigurations
                {
                    ClientId = GetRandomString(),
                    ClientSecret = GetRandomString(),
                    RedirectUri = "https://localhost:5174/auth/callback",
                    AuthEndpoint = $"{baseUrl}{AuthorizePath}",
                    TokenEndpoint = $"{baseUrl}{TokenPath}",
                    UserInfoEndpoint = $"{baseUrl}{UserInfoPath}"
                },

                PersonalDemographicsService = new PersonalDemographicsServiceConfigurations
                {
                    BaseUrl = $"{baseUrl}{FhirPath}"
                }
            };

            this.apiPlatformClient = new ApiPlatformClient(this.apiPlatformConfigurations);
            this.careIdentityServiceClient = this.apiPlatformClient.CareIdentityServiceClient;

            this.personalDemographicsServiceClient =
                this.apiPlatformClient.PersonalDemographicsServiceClient;
        }

        private void GivenTokenEndpointReturns(
            string accessToken,
            string refreshToken,
            int expiresInSeconds = 3600)
        {
            var tokenPayload = new Dictionary<string, string>
            {
                ["access_token"] = accessToken,
                ["token_type"] = "Bearer",
                ["expires_in"] = expiresInSeconds.ToString(),
                ["refresh_token"] = refreshToken,
                ["refresh_token_expires_in"] = (expiresInSeconds * 2).ToString()
            };

            this.wireMockServer
                .Given(Request.Create().WithPath(TokenPath).UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(tokenPayload)));
        }

        private void GivenTokenEndpointFailsWith(HttpStatusCode statusCode)
        {
            this.wireMockServer
                .Given(Request.Create().WithPath(TokenPath).UsingPost())
                .RespondWith(Response.Create().WithStatusCode(statusCode));
        }

        private void GivenUserInfoEndpointReturns(string userUid, string roleId)
        {
            string userInfoJson = JsonSerializer.Serialize(new
            {
                nhsid_useruid = userUid,
                name = GetRandomString(),
                sub = GetRandomString(),
                nhsid_nrbac_roles = new[]
                {
                    new
                    {
                        person_orgid = GetRandomString(),
                        person_roleid = roleId,
                        org_code = GetRandomString(),
                        role_name = GetRandomString(),
                        role_code = GetRandomString()
                    }
                }
            });

            this.wireMockServer
                .Given(Request.Create().WithPath(UserInfoPath).UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(userInfoJson));
        }

        private void GivenPatientEndpointReturns(string nhsNumber, string body)
        {
            this.wireMockServer
                .Given(Request.Create().WithPath($"{FhirPath}/Patient/{nhsNumber}").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/fhir+json")
                    .WithBody(body));
        }

        private void GivenPatientEndpointFailsWith(string nhsNumber, HttpStatusCode statusCode)
        {
            this.wireMockServer
                .Given(Request.Create().WithPath($"{FhirPath}/Patient/{nhsNumber}").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(statusCode));
        }

        private static string ExtractStateFromLoginUrl(string loginUrl)
        {
            var uri = new Uri(loginUrl);
            string query = uri.Query.TrimStart('?');

            foreach (string pair in query.Split('&'))
            {
                string[] parts = pair.Split('=');

                if (parts.Length == 2 && parts[0] == "state")
                {
                    return parts[1];
                }
            }

            return string.Empty;
        }

        private static SearchCriteria CreateSearchCriteriaByNhsNumber(string nhsNumber) =>
            new SearchCriteria
            {
                NhsNumber = nhsNumber
            };

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 8, wordMaxLength: 12).GetValue();

        private static string GetRandomNhsNumber() =>
            new IntRange(min: 100000000, max: 999999999).GetValue().ToString();

        public void Dispose()
        {
            this.wireMockServer.Stop();
            this.wireMockServer.Dispose();
        }
    }
}
