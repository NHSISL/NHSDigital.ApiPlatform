// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xeptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowClientValidationExceptionOnBuildLoginUrlIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            var expectedException =
                new CareIdentityServiceClientValidationException(
                    message: "Care identity service client validation error occurred, fix errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<string> buildLoginUrlTask = this.careIdentityServiceClient.BuildLoginUrlAsync();

            CareIdentityServiceClientValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientValidationException>(
                    async () => await buildLoginUrlTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowClientDependencyValidationExceptionOnLogoutIfDependencyValidationErrorOccursAsync(
            Xeption dependencyValidationException)
        {
            // given
            var expectedException =
                new CareIdentityServiceClientDependencyValidationException(
                    message: "Care identity service client validation error occurred, fix errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask logoutTask = this.careIdentityServiceClient.LogoutAsync();

            CareIdentityServiceClientDependencyValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyValidationException>(
                    async () => await logoutTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowClientDependencyExceptionOnGetAccessTokenIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            var expectedException =
                new CareIdentityServiceClientDependencyException(
                    message: "Care identity service client dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<string> getAccessTokenTask = this.careIdentityServiceClient.GetAccessTokenAsync();

            CareIdentityServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyException>(
                    async () => await getAccessTokenTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowClientServiceExceptionOnGetUserInfoIfServiceErrorOccursAsync(
            Xeption serviceException)
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();

            var expectedException =
                new CareIdentityServiceClientServiceException(
                    message: "Care identity service client service error occurred, contact support.",
                    innerException: serviceException.InnerException as Xeption);

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetUserInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityServiceClient.GetUserInfoAsync(randomCode, randomState);

            CareIdentityServiceClientServiceException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientServiceException>(
                    async () => await getUserInfoTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Theory]
        [MemberData(nameof(UnexpectedExceptions))]
        public async Task ShouldThrowClientServiceExceptionOnBuildLoginUrlIfUnexpectedErrorOccursAsync(
            Exception unexpectedException)
        {
            // given
            var failedCareIdentityServiceClientException =
                new FailedCareIdentityServiceClientException(
                    message: "Unexpected error occurred, contact support.",
                    innerException: unexpectedException,
                    data: unexpectedException.Data);

            var expectedException =
                new CareIdentityServiceClientServiceException(
                    message: "Care identity service client service error occurred, contact support.",
                    innerException: failedCareIdentityServiceClientException);

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(unexpectedException);

            // when
            ValueTask<string> buildLoginUrlTask = this.careIdentityServiceClient.BuildLoginUrlAsync();

            CareIdentityServiceClientServiceException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientServiceException>(
                    async () => await buildLoginUrlTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }
    }
}
