// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Orchestrations.Pds
{
    public partial class PdsOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfSearchCriteriaIsNullAsync()
        {
            // given
            SearchCriteria nullSearchCriteria = null;

            var nullSearchCriteriaPdsOrchestrationException =
                new NullSearchCriteriaPdsOrchestrationException(
                    message: "Search criteria is null.");

            var expectedPdsOrchestrationValidationException =
                new PdsOrchestrationValidationException(
                    message: "PDS orchestration validation error occurred, fix the errors and try again.",
                    innerException: nullSearchCriteriaPdsOrchestrationException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(nullSearchCriteria);

            PdsOrchestrationValidationException actualPdsOrchestrationValidationException =
                await Assert.ThrowsAsync<PdsOrchestrationValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPdsOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedPdsOrchestrationValidationException))),
                    Times.Once);

            this.careIdentityServiceMock.Verify(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfNhsNumberAndSurnameAreMissingAsync()
        {
            // given
            var emptySearchCriteria = new SearchCriteria();

            var invalidArgumentPdsOrchestrationException =
                new InvalidArgumentPdsOrchestrationException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentPdsOrchestrationException.UpsertDataList(
                key: "searchCriteria",
                value: "Either an NHS number or a surname is required");

            var expectedPdsOrchestrationValidationException =
                new PdsOrchestrationValidationException(
                    message: "PDS orchestration validation error occurred, fix the errors and try again.",
                    innerException: invalidArgumentPdsOrchestrationException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(emptySearchCriteria);

            PdsOrchestrationValidationException actualPdsOrchestrationValidationException =
                await Assert.ThrowsAsync<PdsOrchestrationValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPdsOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedPdsOrchestrationValidationException))),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfAccessTokenIsUnavailableAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            var unauthorizedPdsOrchestrationException =
                new UnauthorizedPdsOrchestrationException(
                    message: "Unauthorized - Unable to retrieve access token.");

            var expectedPdsOrchestrationValidationException =
                new PdsOrchestrationValidationException(
                    message: "PDS orchestration validation error occurred, fix the errors and try again.",
                    innerException: unauthorizedPdsOrchestrationException);

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(string.Empty);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria);

            PdsOrchestrationValidationException actualPdsOrchestrationValidationException =
                await Assert.ThrowsAsync<PdsOrchestrationValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPdsOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedPdsOrchestrationValidationException))),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.SearchPatientsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchCriteria>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }
    }
}
