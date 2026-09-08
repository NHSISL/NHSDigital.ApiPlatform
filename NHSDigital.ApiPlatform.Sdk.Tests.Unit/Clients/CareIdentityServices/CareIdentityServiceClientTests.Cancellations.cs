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
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        [Fact]
        public async Task ShouldRethrowOperationCanceledExceptionOnBuildLoginUrlIfCancellationRequestedAsync()
        {
            // given
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<string> buildLoginUrlTask =
                this.careIdentityServiceClient.BuildLoginUrlAsync(cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await buildLoginUrlTask);
        }

        [Fact]
        public async Task ShouldRethrowOperationCanceledExceptionOnLogoutIfCancellationRequestedAsync()
        {
            // given
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask logoutTask =
                this.careIdentityServiceClient.LogoutAsync(cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await logoutTask);
        }

        [Fact]
        public async Task ShouldRethrowOperationCanceledExceptionOnGetUserInfoIfCancellationRequestedAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetUserInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityServiceClient.GetUserInfoAsync(
                    randomCode,
                    randomState,
                    cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await getUserInfoTask);
        }

        [Fact]
        public async Task ShouldRethrowCancellationRaisedByTheProcessingServiceOnGetAccessTokenAsync()
        {
            // given
            // The token is live when the call starts; the dependency is what raises the cancellation.
            using var cancellationTokenSource = new CancellationTokenSource();

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .Callback(() => cancellationTokenSource.Cancel())
                    .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<string> getAccessTokenTask =
                this.careIdentityServiceClient.GetAccessTokenAsync(cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await getAccessTokenTask);
        }

        [Fact]
        public async Task ShouldSurfaceAProcessingTimeoutAsClientDependencyExceptionOnBuildLoginUrlAsync()
        {
            // given
            var timeoutException = new TimeoutException("The dependency operation timed out.");

            var timeoutCareIdentityServiceException =
                new TimeoutCareIdentityServiceException(
                    message: "Failed care identity service timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var processingDependencyException =
                new CareIdentityServiceProcessingDependencyException(
                    message: "Care identity service processing dependency error occurred, please contact support.",
                    innerException: timeoutCareIdentityServiceException);

            var expectedException =
                new CareIdentityServiceClientDependencyException(
                    message: "Care identity service client dependency error occurred, contact support.",
                    innerException: timeoutCareIdentityServiceException);

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(processingDependencyException);

            // when
            ValueTask<string> buildLoginUrlTask = this.careIdentityServiceClient.BuildLoginUrlAsync();

            CareIdentityServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyException>(
                    async () => await buildLoginUrlTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();
        }

        [Fact]
        public async Task ShouldPropagateCancellationTokenToProcessingServiceOnGetUserInfoAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetUserInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateRandomNhsUserInfo());

            // when
            await this.careIdentityServiceClient.GetUserInfoAsync(randomCode, randomState, cancellationToken);

            // then
            this.careIdentityServiceProcessingServiceMock.Verify(service =>
                service.GetUserInfoAsync(randomCode, randomState, cancellationToken),
                    Times.Once);
        }
    }
}
