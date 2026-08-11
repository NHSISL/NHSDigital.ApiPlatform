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
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.PersonalDemographicsServices
{
    public partial class PersonalDemographicsServiceClientTests
    {
        [Fact]
        public async Task ShouldThrowClientDependencyExceptionOnSearchPatientsIfOperationCanceledExceptionOccursAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            var operationCanceledException = new OperationCanceledException();

            PersonalDemographicsServiceClientDependencyException expectedException =
                CreateExpectedTimeoutDependencyException();

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            PersonalDemographicsServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Fact]
        public async Task ShouldThrowClientDependencyExceptionOnSearchPatientsIfTaskCanceledExceptionOccursAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            var taskCanceledException = new TaskCanceledException();

            PersonalDemographicsServiceClientDependencyException expectedException =
                CreateExpectedTimeoutDependencyException();

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(taskCanceledException);

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            PersonalDemographicsServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnSearchPatientsIfCancellationRequestedAsync()
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
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);
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

        private static PersonalDemographicsServiceClientDependencyException
            CreateExpectedTimeoutDependencyException()
        {
            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutPersonalDemographicsServiceClientException =
                new TimeoutPersonalDemographicsServiceClientException(
                    message: "Failed personal demographics service client timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            return new PersonalDemographicsServiceClientDependencyException(
                message: "Personal demographics service client dependency error occurred, contact support.",
                innerException: timeoutPersonalDemographicsServiceClientException);
        }
    }
}
