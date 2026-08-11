// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Orchestrations.Pds
{
    public partial class PdsOrchestrationServiceTests
    {
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
        public async Task ShouldRethrowOperationCanceledExceptionRaisedByADependencyOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            // The token is live when the call starts, so ThrowIfCancellationRequested lets us through and the
            // dependency itself is the one that raises the cancellation.
            using var cancellationTokenSource = new CancellationTokenSource();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .Callback(() => cancellationTokenSource.Cancel())
                    .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<string> searchPatientsTask = this.pdsOrchestrationService.SearchPatientsAsync(
                randomSearchCriteria,
                cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);

            this.careIdentityServiceMock.Verify(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldSurfaceAFoundationTimeoutAsDependencyExceptionOnSearchPatientsAndLogItAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            var timeoutException = new TimeoutException("The dependency operation timed out.");

            var timeoutPdsServiceException =
                new TimeoutPdsServiceException(
                    message: "Failed PDS service timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var pdsServiceDependencyException =
                new PdsServiceDependencyException(
                    message: "PDS service dependency error occurred, please contact support.",
                    innerException: timeoutPdsServiceException);

            var expectedException =
                new PdsOrchestrationDependencyException(
                    message: "PDS orchestration dependency error occurred, please contact support.",
                    innerException: timeoutPdsServiceException);

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetRandomString());

            this.pdsServiceMock.Setup(service =>
                service.SearchPatientsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchCriteria>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(pdsServiceDependencyException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria);

            PdsOrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<PdsOrchestrationDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);
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
    }
}
