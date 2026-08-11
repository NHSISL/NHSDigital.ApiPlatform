// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Brokers.Https;
using NHSDigital.ApiPlatform.Sdk.Brokers.Identifiers;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds;
using Tynamix.ObjectFiller;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        private readonly Mock<IHttpBroker> httpBrokerMock;
        private readonly Mock<IIdentifierBroker> identifierBrokerMock;
        private readonly Mock<IApiPlatformTokenBroker> tokenBrokerMock;
        private readonly ApiPlatformConfigurations apiPlatformConfigurations;
        private readonly IPdsService pdsService;

        public PdsServiceTests()
        {
            this.httpBrokerMock = new Mock<IHttpBroker>();
            this.identifierBrokerMock = new Mock<IIdentifierBroker>();
            this.tokenBrokerMock = new Mock<IApiPlatformTokenBroker>();

            this.apiPlatformConfigurations = new ApiPlatformConfigurations
            {
                PersonalDemographicsService = new PersonalDemographicsServiceConfigurations
                {
                    BaseUrl = $"https://{GetRandomString()}/fhir"
                }
            };

            this.pdsService = new PdsService(
                configurations: this.apiPlatformConfigurations,
                httpBroker: this.httpBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,
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

        private static SearchCriteria CreateRandomSearchCriteriaWithNhsNumber() =>
            new SearchCriteria
            {
                NhsNumber = GetRandomString()
            };

        private static SearchCriteria CreateRandomSearchCriteriaWithDemographics() =>
            new SearchCriteria
            {
                Surname = GetRandomString(),
                FirstName = GetRandomString(),
                Gender = GetRandomString(),
                DateOfBirth = "1980-01-01",
                Postcode = GetRandomString()
            };

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 8, wordMaxLength: 12).GetValue();

        private static HttpResponseMessage CreateHttpResponse(string content) =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };

        private static PdsServiceDependencyException CreateExpectedDependencyException(
            Exception dependencyException)
        {
            var failedPdsServiceDependencyException =
                new FailedPdsServiceDependencyException(
                    message: "Failed PDS service dependency error occurred, please contact support.",
                    innerException: dependencyException);

            return new PdsServiceDependencyException(
                message: "PDS service dependency error occurred, please contact support.",
                innerException: failedPdsServiceDependencyException);
        }

        private static PdsServiceException CreateExpectedServiceException(Exception serviceException)
        {
            var failedPdsServiceException =
                new FailedPdsServiceException(
                    message: "Failed PDS service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            return new PdsServiceException(
                message: "PDS service error occurred, please contact support.",
                innerException: failedPdsServiceException);
        }
    }
}
