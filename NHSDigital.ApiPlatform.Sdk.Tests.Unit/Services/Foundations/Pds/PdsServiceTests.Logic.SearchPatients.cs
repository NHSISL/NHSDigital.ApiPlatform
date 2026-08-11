// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldReturnPatientPayloadOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string randomPayload = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(randomPayload));

            // when
            string actualPayload = await this.pdsService.SearchPatientsAsync(
                randomAccessToken,
                randomSearchCriteria);

            // then
            actualPayload.Should().Be(randomPayload);
        }

        [Fact]
        public async Task ShouldSearchByNhsNumberOnSearchPatientsIfNhsNumberIsSuppliedAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            string expectedUrl =
                $"{this.apiPlatformConfigurations.PersonalDemographicsService.BaseUrl}" +
                $"/Patient/{randomSearchCriteria.NhsNumber}";

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(GetRandomString()));

            // when
            await this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            // then
            this.httpBrokerMock.Verify(broker =>
                broker.GetAsync(
                    expectedUrl,
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldSearchByDemographicsOnSearchPatientsIfNhsNumberIsMissingAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithDemographics();

            string expectedUrl =
                $"{this.apiPlatformConfigurations.PersonalDemographicsService.BaseUrl}" +
                $"/Patient?family={Uri.EscapeDataString(randomSearchCriteria.Surname)}" +
                $"&given={Uri.EscapeDataString(randomSearchCriteria.FirstName)}" +
                $"&gender={Uri.EscapeDataString(randomSearchCriteria.Gender)}" +
                $"&birthdate=eq{randomSearchCriteria.DateOfBirth}" +
                $"&address-postalcode={Uri.EscapeDataString(randomSearchCriteria.Postcode)}";

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(GetRandomString()));

            // when
            await this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            // then
            this.httpBrokerMock.Verify(broker =>
                broker.GetAsync(
                    expectedUrl,
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldRetrieveActiveRoleOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(GetRandomString()));

            // when
            await this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.GetActiveRoleAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldAddAuthorisationAndTracingHeadersOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string randomActiveRoleId = GetRandomString();
            Guid randomRequestId = Guid.NewGuid();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();
            var actualRequest = new HttpRequestMessage(HttpMethod.Get, "https://localhost/patient");

            this.tokenBrokerMock.Setup(broker =>
                broker.GetActiveRoleAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomActiveRoleId);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetNewGuid())
                    .Returns(randomRequestId);

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Callback<string, Action<HttpRequestMessage>, CancellationToken>(
                            (url, configureRequest, cancellationToken) => configureRequest(actualRequest))
                        .ReturnsAsync(CreateHttpResponse(GetRandomString()));

            // when
            await this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            // then
            actualRequest.Headers.Authorization.Parameter.Should().Be(randomAccessToken);
            actualRequest.Headers.GetValues("X-Request-ID").Should().Contain(randomRequestId.ToString());
            actualRequest.Headers.GetValues("NHSD-Session-URID").Should().Contain(randomActiveRoleId);
        }

        [Fact]
        public async Task ShouldNotAddSessionRoleHeaderOnSearchPatientsIfActiveRoleIsMissingAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();
            var actualRequest = new HttpRequestMessage(HttpMethod.Get, "https://localhost/patient");

            this.tokenBrokerMock.Setup(broker =>
                broker.GetActiveRoleAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((string)null);

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Callback<string, Action<HttpRequestMessage>, CancellationToken>(
                            (url, configureRequest, cancellationToken) => configureRequest(actualRequest))
                        .ReturnsAsync(CreateHttpResponse(GetRandomString()));

            // when
            await this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            // then
            actualRequest.Headers.Contains("NHSD-Session-URID").Should().BeFalse();
        }
    }
}
