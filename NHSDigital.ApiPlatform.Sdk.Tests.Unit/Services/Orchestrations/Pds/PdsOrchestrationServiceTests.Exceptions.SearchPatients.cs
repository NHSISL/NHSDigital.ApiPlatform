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
using Xeptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Orchestrations.Pds
{
    public partial class PdsOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnSearchPatientsIfValidationErrorOccursAsync(
            Xeption dependencyValidationException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            var expectedPdsOrchestrationDependencyValidationException =
                new PdsOrchestrationDependencyValidationException(
                    message: "PDS orchestration dependency validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria);

            PdsOrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<PdsOrchestrationDependencyValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPdsOrchestrationDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedPdsOrchestrationDependencyValidationException))),
                    Times.Once);
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            var expectedPdsOrchestrationDependencyException =
                new PdsOrchestrationDependencyException(
                    message: "PDS orchestration dependency error occurred, please contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria);

            PdsOrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<PdsOrchestrationDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPdsOrchestrationDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedPdsOrchestrationDependencyException))),
                    Times.Once);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowServiceExceptionOnSearchPatientsIfServiceErrorOccursAsync(
            Exception serviceException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            PdsOrchestrationServiceException expectedPdsOrchestrationServiceException =
                CreateExpectedServiceException(serviceException);

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria);

            PdsOrchestrationServiceException actualPdsOrchestrationServiceException =
                await Assert.ThrowsAsync<PdsOrchestrationServiceException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsOrchestrationServiceException
                .Should().BeEquivalentTo(expectedPdsOrchestrationServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedPdsOrchestrationServiceException))),
                    Times.Once);
        }

    }
}
