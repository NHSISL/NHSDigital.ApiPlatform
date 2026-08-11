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
        public async Task ShouldThrowDependencyValidationExceptionOnSearchPatientsIfDependencyValidationErrorOccursAsync(
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
                    message: "PDS orchestration dependency error occurred, fix the errors and try again.",
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
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfTimeoutOccursInFoundationServiceAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            var timeoutException = new TimeoutException();

            var failedPdsServiceDependencyException =
                new NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions
                    .FailedPdsServiceDependencyException(
                        message: "Failed PDS service dependency error occurred, please contact support.",
                        innerException: timeoutException);

            var pdsServiceDependencyException =
                new NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions.PdsServiceDependencyException(
                    message: "PDS service dependency error occurred, please contact support.",
                    innerException: failedPdsServiceDependencyException);

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
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();
        }
    }
}
