// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldEscapeTheNhsNumberOnSearchPatientsSoItCannotEscapeThePatientPathAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string actualUrl = null;

            // A caller-supplied NHS number must never be able to steer the request somewhere else on the
            // authenticated host, nor bolt extra FHIR query parameters onto it.
            var traversalSearchCriteria = new SearchCriteria
            {
                NhsNumber = "../../Practitioner/1?scope=all"
            };

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Callback<string, Action<HttpRequestMessage>, CancellationToken>(
                            (url, configureRequest, cancellationToken) => actualUrl = url)
                        .ReturnsAsync(CreateHttpResponse(GetRandomString()));

            // when
            await this.pdsService.SearchPatientsAsync(randomAccessToken, traversalSearchCriteria);

            // then
            string expectedBaseUrl =
                $"{this.apiPlatformConfigurations.PersonalDemographicsService.BaseUrl}/Patient/";

            actualUrl.Should().StartWith(expectedBaseUrl);
            actualUrl.Should().NotContain("/Practitioner");
            actualUrl.Should().NotContain("?");
            actualUrl.Substring(expectedBaseUrl.Length).Should().NotContain("/");
        }

        [Fact]
        public async Task ShouldEscapeDemographicSearchValuesOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string actualUrl = null;

            var searchCriteria = new SearchCriteria
            {
                Surname = "O'Brien & Sons",
                FirstName = "Anne Marie",
                Gender = "female",
                Postcode = "E1 6AN"
            };

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Callback<string, Action<HttpRequestMessage>, CancellationToken>(
                            (url, configureRequest, cancellationToken) => actualUrl = url)
                        .ReturnsAsync(CreateHttpResponse(GetRandomString()));

            // when
            await this.pdsService.SearchPatientsAsync(randomAccessToken, searchCriteria);

            // then
            actualUrl.Should().Contain($"family={Uri.EscapeDataString("O'Brien & Sons")}");
            actualUrl.Should().Contain($"given={Uri.EscapeDataString("Anne Marie")}");
            actualUrl.Should().Contain($"address-postalcode={Uri.EscapeDataString("E1 6AN")}");

            // The raw ampersand would otherwise have introduced a query parameter of its own.
            actualUrl.Should().NotContain("& Sons");
            actualUrl.Should().NotContain("Anne Marie");
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.NotFound)]
        public async Task ShouldThrowDependencyValidationExceptionOnSearchPatientsIfPdsRejectsTheRequestAsync(
            HttpStatusCode statusCode)
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new HttpResponseMessage(statusCode)
                        {
                            Content = new StringContent(GetRandomString())
                        });

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            PdsServiceDependencyValidationException actualException =
                await Assert.ThrowsAsync<PdsServiceDependencyValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualException.InnerException.Should().BeOfType<InvalidPdsServiceDependencyException>();
            actualException.InnerException.InnerException.Should().BeOfType<HttpRequestException>();
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfPdsFailsAsync(HttpStatusCode statusCode)
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new HttpResponseMessage(statusCode)
                        {
                            Content = new StringContent(GetRandomString())
                        });

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            PdsServiceDependencyException actualException =
                await Assert.ThrowsAsync<PdsServiceDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualException.InnerException.Should().BeOfType<FailedPdsServiceDependencyException>();
        }
    }
}
