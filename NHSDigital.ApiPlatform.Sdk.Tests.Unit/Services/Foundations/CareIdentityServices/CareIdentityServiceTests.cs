// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Brokers.Cryptographies;
using NHSDigital.ApiPlatform.Sdk.Brokers.DateTimes;
using NHSDigital.ApiPlatform.Sdk.Brokers.Https;
using NHSDigital.ApiPlatform.Sdk.Brokers.Serializations;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices;
using Tynamix.ObjectFiller;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.CareIdentityServices
{
    public partial class CareIdentityServiceTests
    {
        private readonly Mock<IHttpBroker> httpBrokerMock;
        private readonly Mock<IJsonBroker> jsonBrokerMock;
        private readonly Mock<ICryptoBroker> cryptoBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<IApiPlatformStateBroker> stateBrokerMock;
        private readonly Mock<IApiPlatformTokenBroker> tokenBrokerMock;
        private readonly ApiPlatformConfigurations apiPlatformConfigurations;
        private readonly ICareIdentityService careIdentityService;

        public CareIdentityServiceTests()
        {
            this.httpBrokerMock = new Mock<IHttpBroker>();
            this.jsonBrokerMock = new Mock<IJsonBroker>();
            this.cryptoBrokerMock = new Mock<ICryptoBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.stateBrokerMock = new Mock<IApiPlatformStateBroker>();
            this.tokenBrokerMock = new Mock<IApiPlatformTokenBroker>();
            this.apiPlatformConfigurations = CreateRandomConfigurations();

            this.careIdentityService = new CareIdentityService(
                configurations: this.apiPlatformConfigurations,
                httpBroker: this.httpBrokerMock.Object,
                jsonBroker: this.jsonBrokerMock.Object,
                cryptoBroker: this.cryptoBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                stateBroker: this.stateBrokerMock.Object,
                tokenBroker: this.tokenBrokerMock.Object);
        }

        public static TheoryData<Exception> DependencyExceptions() =>
            new TheoryData<Exception>
            {
                new HttpRequestException(),
                new TimeoutException()
            };

        public static TheoryData<Exception> ServiceExceptions() =>
            new TheoryData<Exception>
            {
                new Exception(),
                new InvalidOperationException(),
                new NotSupportedException()
            };

        public static TheoryData<string> InvalidTexts() =>
            new TheoryData<string>
            {
                null,
                string.Empty,
                " "
            };

        private static ApiPlatformConfigurations CreateRandomConfigurations() =>
            new ApiPlatformConfigurations
            {
                CareIdentity = new CareIdentityConfigurations
                {
                    ClientId = GetRandomString(),
                    ClientSecret = GetRandomString(),
                    RedirectUri = $"https://{GetRandomString()}/callback",
                    AuthEndpoint = $"https://{GetRandomString()}/authorize",
                    TokenEndpoint = $"https://{GetRandomString()}/token",
                    UserInfoEndpoint = $"https://{GetRandomString()}/userinfo",
                    AcrValues = null
                },

                PersonalDemographicsService = new PersonalDemographicsServiceConfigurations
                {
                    BaseUrl = $"https://{GetRandomString()}/fhir"
                }
            };

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 8, wordMaxLength: 12).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 100, max: 900).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static HttpResponseMessage CreateHttpResponse(string content) =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };

        private static TokenResult CreateRandomTokenResult() =>
            new TokenResult
            {
                AccessToken = GetRandomString(),
                RefreshToken = GetRandomString(),
                TokenType = "Bearer",
                ExpiresIn = GetRandomNumber().ToString(),
                RefreshTokenExpiresIn = GetRandomNumber().ToString()
            };

        private static NhsUserInfo CreateRandomNhsUserInfo() =>
            new NhsUserInfo
            {
                NhsIdUserUid = GetRandomString(),
                Name = GetRandomString(),
                Sub = GetRandomString(),
                NhsIdNrbacRoles = new List<NhsNrbacRole>
                {
                    new NhsNrbacRole
                    {
                        PersonRoleId = GetRandomString(),
                        PersonOrgId = GetRandomString(),
                        OrgCode = GetRandomString(),
                        RoleName = GetRandomString(),
                        RoleCode = GetRandomString()
                    }
                }
            };
    }
}
