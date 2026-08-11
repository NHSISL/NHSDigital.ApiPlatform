// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Processings.CareIdentityServices
{
    public partial class CareIdentityServiceProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnBuildLoginUrlIfCancellationRequestedAsync()
        {
            // given
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask<string> buildLoginUrlTask =
                this.careIdentityServiceProcessingService.BuildLoginUrlAsync(cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await buildLoginUrlTask);

            this.careIdentityServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnLogoutIfCancellationRequestedAsync()
        {
            // given
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask logoutTask =
                this.careIdentityServiceProcessingService.LogoutAsync(cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await logoutTask);

            this.careIdentityServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnGetAccessTokenIfCancellationRequestedAsync()
        {
            // given
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask<string> getAccessTokenTask =
                this.careIdentityServiceProcessingService.GetAccessTokenAsync(cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await getAccessTokenTask);

            this.careIdentityServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnGetUserInfoIfCancellationRequestedAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityServiceProcessingService.GetUserInfoAsync(
                    randomCode,
                    randomState,
                    cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await getUserInfoTask);

            this.careIdentityServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRethrowOperationCanceledExceptionRaisedByTheFoundationOnGetAccessTokenAsync()
        {
            // given
            // The token is live when the call starts, so ThrowIfCancellationRequested lets us through and the
            // dependency itself is the one that raises the cancellation.
            using var cancellationTokenSource = new CancellationTokenSource();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .Callback(() => cancellationTokenSource.Cancel())
                    .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<string> getAccessTokenTask =
                this.careIdentityServiceProcessingService.GetAccessTokenAsync(cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await getAccessTokenTask);

            this.careIdentityServiceMock.Verify(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldSurfaceAFoundationTimeoutAsDependencyExceptionOnBuildLoginUrlAndLogItAsync()
        {
            // given
            var timeoutException = new TimeoutException("The dependency operation timed out.");

            var timeoutCareIdentityServiceException =
                new TimeoutCareIdentityServiceException(
                    message: "Failed care identity service timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var careIdentityServiceDependencyException =
                new CareIdentityServiceDependencyException(
                    message: "Care identity service dependency error occurred, please contact support.",
                    innerException: timeoutCareIdentityServiceException);

            var expectedException =
                new CareIdentityServiceProcessingDependencyException(
                    message: "Care identity service processing dependency error occurred, please contact support.",
                    innerException: timeoutCareIdentityServiceException);

            this.careIdentityServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(careIdentityServiceDependencyException);

            // when
            ValueTask<string> buildLoginUrlTask =
                this.careIdentityServiceProcessingService.BuildLoginUrlAsync();

            CareIdentityServiceProcessingDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceProcessingDependencyException>(
                    async () => await buildLoginUrlTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldPropagateCancellationTokenToFoundationServiceOnGetUserInfoAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            string randomAccessToken = GetRandomString();
            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomAccessToken);

            this.careIdentityServiceMock.Setup(service =>
                service.GetUserInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(CreateRandomNhsUserInfo());

            // when
            await this.careIdentityServiceProcessingService.GetUserInfoAsync(
                randomCode,
                randomState,
                cancellationToken);

            // then
            this.careIdentityServiceMock.Verify(service =>
                service.CallbackAsync(randomCode, randomState, cancellationToken),
                    Times.Once);

            this.careIdentityServiceMock.Verify(service =>
                service.GetUserInfoAsync(randomAccessToken, cancellationToken),
                    Times.Once);
        }
    }
}
