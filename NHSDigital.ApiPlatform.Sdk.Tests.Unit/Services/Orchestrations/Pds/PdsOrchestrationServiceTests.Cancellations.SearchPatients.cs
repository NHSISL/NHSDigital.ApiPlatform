// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfOperationCanceledOccursAndLogItAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            var operationCanceledException = new OperationCanceledException();

            PdsOrchestrationDependencyException expectedPdsOrchestrationDependencyException =
                CreateExpectedTimeoutDependencyException();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria);

            PdsOrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<PdsOrchestrationDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPdsOrchestrationDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedPdsOrchestrationDependencyException))),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnSearchPatientsIfCancellationRequestedAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask<string> searchPatientsTask = this.pdsOrchestrationService.SearchPatientsAsync(
                randomSearchCriteria,
                cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);

            this.careIdentityServiceMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionRaisedByADependencyOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(
                    randomSearchCriteria,
                    cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPropagateCancellationTokenToDependenciesOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            string randomAccessToken = GetRandomString();
            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomAccessToken);

            this.pdsServiceMock.Setup(service =>
                service.SearchPatientsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchCriteria>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(GetRandomString());

            // when
            await this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria, cancellationToken);

            // then
            this.careIdentityServiceMock.Verify(service =>
                service.GetAccessTokenAsync(cancellationToken),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.SearchPatientsAsync(randomAccessToken, randomSearchCriteria, cancellationToken),
                    Times.Once);
        }

        private static PdsOrchestrationDependencyException CreateExpectedTimeoutDependencyException()
        {
            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutPdsOrchestrationException =
                new TimeoutPdsOrchestrationException(
                    message: "Failed PDS orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            return new PdsOrchestrationDependencyException(
                message: "PDS orchestration dependency error occurred, fix the errors and try again.",
                innerException: timeoutPdsOrchestrationException);
        }
    }
}
