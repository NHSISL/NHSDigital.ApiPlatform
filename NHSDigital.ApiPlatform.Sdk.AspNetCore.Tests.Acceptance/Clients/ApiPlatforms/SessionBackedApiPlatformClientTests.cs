// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class SessionBackedApiPlatformClientTests : IDisposable
    {
        private const string TokenPath = "/oauth2/token";
        private const string UserInfoPath = "/oauth2/userinfo";
        private const string AuthorizePath = "/oauth2/authorize";
        private const string FhirPath = "/personal-demographics/FHIR/R4";

        private readonly WireMockServer wireMockServer;
        private readonly ApiPlatformConfigurations apiPlatformConfigurations;
        private readonly ServiceProvider serviceProvider;
        private readonly IServiceScope serviceScope;
        private readonly FakeSession fakeSession;
        private readonly ICareIdentityServiceClient careIdentityServiceClient;
        private readonly IPersonalDemographicsServiceClient personalDemographicsServiceClient;

        public SessionBackedApiPlatformClientTests()
        {
            this.wireMockServer = WireMockServer.Start();
            string baseUrl = this.wireMockServer.Urls[0];
            this.fakeSession = new FakeSession();

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

            var httpContext = new DefaultHttpContext
            {
                Session = this.fakeSession
            };

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor { HttpContext = httpContext });

            services.AddApiPlatformSdkCore(this.apiPlatformConfigurations);
            services.AddApiPlatformSdkAspNetCore();

            // Keep the dependency timeout short so the timeout path is observable in a test run.
            services.AddHttpClient("NhsApiPlatform")
                .ConfigureHttpClient(httpClient => httpClient.Timeout = TimeSpan.FromMilliseconds(500));

            this.serviceProvider = services.BuildServiceProvider();
            this.serviceScope = this.serviceProvider.CreateScope();

            IApiPlatformClient apiPlatformClient =
                this.serviceScope.ServiceProvider.GetRequiredService<IApiPlatformClient>();

            this.careIdentityServiceClient = apiPlatformClient.CareIdentityServiceClient;
            this.personalDemographicsServiceClient = apiPlatformClient.PersonalDemographicsServiceClient;
        }

        private void GivenTokenEndpointReturns(string accessToken, string refreshToken)
        {
            var tokenPayload = new Dictionary<string, string>
            {
                ["access_token"] = accessToken,
                ["token_type"] = "Bearer",
                ["expires_in"] = "3600",
                ["refresh_token"] = refreshToken,
                ["refresh_token_expires_in"] = "7200"
            };

            this.wireMockServer
                .Given(Request.Create().WithPath(TokenPath).UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(tokenPayload)));
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

        private static string ExtractStateFromLoginUrl(string loginUrl)
        {
            string query = new Uri(loginUrl).Query.TrimStart('?');

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
            this.serviceScope.Dispose();
            this.serviceProvider.Dispose();
            this.wireMockServer.Stop();
            this.wireMockServer.Dispose();
        }
    }
}
