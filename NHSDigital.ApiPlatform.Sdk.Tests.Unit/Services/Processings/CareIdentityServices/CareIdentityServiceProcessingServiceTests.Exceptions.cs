// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using Xeptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Processings.CareIdentityServices
{
    public partial class CareIdentityServiceProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBuildLoginUrlIfDependencyValidationErrorOccursAsync(
            Xeption dependencyValidationException)
        {
            // given
            var expectedException =
                new CareIdentityServiceProcessingDependencyValidationException(
                    message: "Care identity service processing dependency validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: dependencyValidationException.InnerException as Xeption);

            this.careIdentityServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<string> buildLoginUrlTask =
                this.careIdentityServiceProcessingService.BuildLoginUrlAsync();

            CareIdentityServiceProcessingDependencyValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingDependencyValidationException>(
                    async () => await buildLoginUrlTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnBuildLoginUrlIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            var expectedException =
                new CareIdentityServiceProcessingDependencyException(
                    message: "Care identity service processing dependency error occurred, please contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.careIdentityServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<string> buildLoginUrlTask =
                this.careIdentityServiceProcessingService.BuildLoginUrlAsync();

            CareIdentityServiceProcessingDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingDependencyException>(
                    async () => await buildLoginUrlTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowServiceExceptionOnBuildLoginUrlIfServiceErrorOccursAsync(
            Exception serviceException)
        {
            // given
            CareIdentityServiceProcessingServiceException expectedException =
                CreateExpectedServiceException(serviceException);

            this.careIdentityServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<string> buildLoginUrlTask =
                this.careIdentityServiceProcessingService.BuildLoginUrlAsync();

            CareIdentityServiceProcessingServiceException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingServiceException>(
                    async () => await buildLoginUrlTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnLogoutIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            var expectedException =
                new CareIdentityServiceProcessingDependencyException(
                    message: "Care identity service processing dependency error occurred, please contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.careIdentityServiceMock.Setup(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask logoutTask = this.careIdentityServiceProcessingService.LogoutAsync();

            CareIdentityServiceProcessingDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingDependencyException>(
                    async () => await logoutTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowServiceExceptionOnLogoutIfServiceErrorOccursAsync(Exception serviceException)
        {
            // given
            CareIdentityServiceProcessingServiceException expectedException =
                CreateExpectedServiceException(serviceException);

            this.careIdentityServiceMock.Setup(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask logoutTask = this.careIdentityServiceProcessingService.LogoutAsync();

            CareIdentityServiceProcessingServiceException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingServiceException>(
                    async () => await logoutTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnGetAccessTokenIfTimeoutOccursInFoundationServiceAsync()
        {
            // given
            var timeoutException = new TimeoutException();

            var failedCareIdentityServiceDependencyException =
                new FailedCareIdentityServiceDependencyException(
                    message: "Failed care identity service dependency error occurred, please contact support.",
                    innerException: timeoutException);

            var careIdentityServiceDependencyException =
                new CareIdentityServiceDependencyException(
                    message: "Care identity service dependency error occurred, please contact support.",
                    innerException: failedCareIdentityServiceDependencyException);

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(careIdentityServiceDependencyException);

            // when
            ValueTask<string> getAccessTokenTask =
                this.careIdentityServiceProcessingService.GetAccessTokenAsync();

            CareIdentityServiceProcessingDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingDependencyException>(
                    async () => await getAccessTokenTask);

            // then
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();
        }
    }
}
