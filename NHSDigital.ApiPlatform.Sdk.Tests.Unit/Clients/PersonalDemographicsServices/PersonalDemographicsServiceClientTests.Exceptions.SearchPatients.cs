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
using Xeptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.PersonalDemographicsServices
{
    public partial class PersonalDemographicsServiceClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowClientValidationExceptionOnSearchPatientsIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            var expectedException =
                new PersonalDemographicsServiceClientValidationException(
                    message: "Personal demographics service client validation error occurred, " +
                        "fix errors and try again.",

                    innerException: validationException.InnerException as Xeption);

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            PersonalDemographicsServiceClientValidationException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowClientDependencyValidationExceptionOnSearchPatientsAsync(
            Xeption dependencyValidationException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            var expectedException =
                new PersonalDemographicsServiceClientDependencyValidationException(
                    message: "Personal demographics service client dependency validation error occurred, " +
                        "fix errors and try again.",

                    innerException: dependencyValidationException.InnerException as Xeption);

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            PersonalDemographicsServiceClientDependencyValidationException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientDependencyValidationException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowClientDependencyExceptionOnSearchPatientsIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            var expectedException =
                new PersonalDemographicsServiceClientDependencyException(
                    message: "Personal demographics service client dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            PersonalDemographicsServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowClientServiceExceptionOnSearchPatientsIfServiceErrorOccursAsync(
            Xeption serviceException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            var expectedException =
                new PersonalDemographicsServiceClientServiceException(
                    message: "Personal demographics service client service error occurred, contact support.",
                    innerException: serviceException.InnerException as Xeption);

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            PersonalDemographicsServiceClientServiceException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientServiceException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(UnexpectedExceptions))]
        public async Task ShouldThrowClientServiceExceptionOnSearchPatientsIfUnexpectedErrorOccursAsync(
            Exception unexpectedException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            var failedPersonalDemographicsServiceClientException =
                new FailedPersonalDemographicsServiceClientException(
                    message: "Unexpected error occurred, contact support.",
                    innerException: unexpectedException,
                    data: unexpectedException.Data);

            var expectedException =
                new PersonalDemographicsServiceClientServiceException(
                    message: "Personal demographics service client service error occurred, contact support.",
                    innerException: failedPersonalDemographicsServiceClientException);

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(unexpectedException);

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            PersonalDemographicsServiceClientServiceException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientServiceException>(
                    async () => await searchPatientsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }
    }
}
