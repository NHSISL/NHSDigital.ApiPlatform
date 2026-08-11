// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Processings.CareIdentityServices
{
    public partial class CareIdentityServiceProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnBuildLoginUrlIfTokenIsAlreadyCancelledAsync()
        {
            // given
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            ValueTask<string> buildLoginUrlTask =
                this.careIdentityServiceProcessingService.BuildLoginUrlAsync(cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await buildLoginUrlTask);

            this.careIdentityServiceMock.Verify(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnLogoutIfTokenIsAlreadyCancelledAsync()
        {
            // given
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            ValueTask logoutTask =
                this.careIdentityServiceProcessingService.LogoutAsync(cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await logoutTask);

            this.careIdentityServiceMock.Verify(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnGetUserInfoIfTokenIsAlreadyCancelledAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityServiceProcessingService.GetUserInfoAsync(
                    randomCode,
                    randomState,
                    cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await getUserInfoTask);

            this.careIdentityServiceMock.Verify(service =>
                service.CallbackAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnGetAccessTokenAsync()
        {
            // given
            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException());

            // when
            ValueTask<string> getAccessTokenTask =
                this.careIdentityServiceProcessingService.GetAccessTokenAsync();

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await getAccessTokenTask);
        }

        [Fact]
        public async Task ShouldNotWrapTaskCanceledExceptionOnLogoutAsync()
        {
            // given
            this.careIdentityServiceMock.Setup(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new TaskCanceledException());

            // when
            ValueTask logoutTask = this.careIdentityServiceProcessingService.LogoutAsync();

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await logoutTask);
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
