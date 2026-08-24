// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.PersonalDemographicsServices
{
    public partial class PersonalDemographicsServiceClientTests
    {
        [Fact]
        public async Task ShouldRethrowOperationCanceledExceptionOnSearchPatientsIfCancellationRequestedAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(
                    randomSearchCriteria,
                    cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);
        }

        [Fact]
        public async Task ShouldRethrowCancellationRaisedByTheOrchestrationOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            // The token is live when the call starts; the dependency is what raises the cancellation.
            using var cancellationTokenSource = new CancellationTokenSource();

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .Callback(() => cancellationTokenSource.Cancel())
                    .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(
                    randomSearchCriteria,
                    cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);
        }

        [Fact]
        public async Task ShouldSurfaceAnOrchestrationTimeoutAsClientDependencyExceptionOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            var timeoutException = new TimeoutException("The dependency operation timed out.");

            var timeoutPdsServiceException =
                new TimeoutPdsServiceException(
                    message: "Failed PDS service timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var orchestrationDependencyException =
                new PdsOrchestrationDependencyException(
                    message: "PDS orchestration dependency error occurred, please contact support.",
                    innerException: timeoutPdsServiceException);

            var expectedException =
                new PersonalDemographicsServiceClientDependencyException(
                    message: "Personal demographics service client dependency error occurred, contact support.",
                    innerException: timeoutPdsServiceException);

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(orchestrationDependencyException);

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            PersonalDemographicsServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();
        }

        [Fact]
        public async Task ShouldPropagateCancellationTokenToOrchestrationServiceOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetRandomString());

            // when
            await this.personalDemographicsServiceClient.SearchPatientsAsync(
                randomSearchCriteria,
                cancellationToken);

            // then
            this.pdsOrchestrationServiceMock.Verify(service =>
                service.SearchPatientsAsync(randomSearchCriteria, cancellationToken),
                    Times.Once);
        }
    }
}
