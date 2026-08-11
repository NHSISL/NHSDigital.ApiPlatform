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
        public async Task ShouldEscapeDemographicSearchValuesOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string actualUrl = null;

            var searchCriteria = new SearchCriteria
            {
                Surname = "O'Brien & Sons",
                FirstName = "Anne Marie",
                Gender = "female/other",
                DateOfBirth = "1980-01-01 00:00",
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
            actualUrl.Should().Contain($"gender={Uri.EscapeDataString("female/other")}");
            actualUrl.Should().Contain($"birthdate=eq{Uri.EscapeDataString("1980-01-01 00:00")}");
            actualUrl.Should().Contain($"address-postalcode={Uri.EscapeDataString("E1 6AN")}");

            // The raw ampersand would otherwise have introduced a query parameter of its own.
            actualUrl.Should().NotContain("& Sons");
            actualUrl.Should().NotContain("Anne Marie");

            // An unescaped "/" in gender would have introduced a path segment.
            actualUrl.Should().NotContain("female/other");
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

        [Theory]
        [InlineData("..")]
        [InlineData(".")]
        [InlineData("../../Practitioner/1")]
        [InlineData("12345")]
        [InlineData("abcdefghij")]
        [InlineData("123456789 ")]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfNhsNumberIsNotTenDigitsAsync(
            string invalidNhsNumber)
        {
            // given
            string randomAccessToken = GetRandomString();
            var searchCriteria = new SearchCriteria { NhsNumber = invalidNhsNumber };

            var invalidArgumentPdsServiceException =
                new InvalidArgumentPdsServiceException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentPdsServiceException.UpsertDataList(
                key: "searchCriteria.NhsNumber",
                value: "NHS number must be 10 digits");

            var expectedPdsServiceValidationException =
                new PdsServiceValidationException(
                    message: "PDS service validation error occurred, please fix the errors and try again.",
                    innerException: invalidArgumentPdsServiceException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, searchCriteria);

            PdsServiceValidationException actualException =
                await Assert.ThrowsAsync<PdsServiceValidationException>(async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPdsServiceValidationException);

            this.httpBrokerMock.Verify(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }

        [Fact]
        public async Task ShouldAssertTheDependencyValidationMessageOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(GetRandomString())
                        });

            // when
            PdsServiceDependencyValidationException actualException =
                await Assert.ThrowsAsync<PdsServiceDependencyValidationException>(async () =>
                    await this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria));

            // then
            actualException.Message
                .Should().Be("PDS service dependency validation error occurred, fix the errors and try again.");

            actualException.InnerException.Message
                .Should().Be("Invalid PDS service dependency error occurred, fix the errors and try again.");
        }
    }
}
