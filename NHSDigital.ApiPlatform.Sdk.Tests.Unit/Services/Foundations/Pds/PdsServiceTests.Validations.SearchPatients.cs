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
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfSearchCriteriaIsNullAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria nullSearchCriteria = null;

            var nullSearchCriteriaPdsServiceException =
                new NullSearchCriteriaPdsServiceException(
                    message: "Search criteria is null.");

            var expectedPdsServiceValidationException =
                new PdsServiceValidationException(
                    message: "PDS service validation error occurred, please fix the errors and try again.",
                    innerException: nullSearchCriteriaPdsServiceException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, nullSearchCriteria);

            PdsServiceValidationException actualPdsServiceValidationException =
                await Assert.ThrowsAsync<PdsServiceValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsServiceValidationException
                .Should().BeEquivalentTo(expectedPdsServiceValidationException);

            this.httpBrokerMock.Verify(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }

        [Theory]
        [MemberData(nameof(InvalidTexts))]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfAccessTokenIsInvalidAsync(
            string invalidAccessToken)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            var invalidArgumentPdsServiceException =
                new InvalidArgumentPdsServiceException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentPdsServiceException.UpsertDataList(
                key: "accessToken",
                value: "Text is required");

            var expectedPdsServiceValidationException =
                new PdsServiceValidationException(
                    message: "PDS service validation error occurred, please fix the errors and try again.",
                    innerException: invalidArgumentPdsServiceException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(invalidAccessToken, randomSearchCriteria);

            PdsServiceValidationException actualPdsServiceValidationException =
                await Assert.ThrowsAsync<PdsServiceValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsServiceValidationException
                .Should().BeEquivalentTo(expectedPdsServiceValidationException);
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfNhsNumberAndSurnameAreMissingAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            var emptySearchCriteria = new SearchCriteria();

            var invalidArgumentPdsServiceException =
                new InvalidArgumentPdsServiceException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentPdsServiceException.UpsertDataList(
                key: "searchCriteria",
                value: "Either an NHS number or a surname is required");

            var expectedPdsServiceValidationException =
                new PdsServiceValidationException(
                    message: "PDS service validation error occurred, please fix the errors and try again.",
                    innerException: invalidArgumentPdsServiceException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, emptySearchCriteria);

            PdsServiceValidationException actualPdsServiceValidationException =
                await Assert.ThrowsAsync<PdsServiceValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsServiceValidationException
                .Should().BeEquivalentTo(expectedPdsServiceValidationException);
        }
    }
}
