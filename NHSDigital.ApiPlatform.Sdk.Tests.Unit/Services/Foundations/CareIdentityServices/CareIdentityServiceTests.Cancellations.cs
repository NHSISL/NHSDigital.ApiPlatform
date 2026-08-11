// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.CareIdentityServices
{
    public partial class CareIdentityServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnBuildLoginUrlIfCancellationRequestedAsync()
        {
            // given
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask<string> buildLoginUrlTask =
                this.careIdentityService.BuildLoginUrlAsync(cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await buildLoginUrlTask);

            this.cryptoBrokerMock.Verify(broker =>
                broker.CreateUrlSafeState(It.IsAny<int>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnLogoutIfCancellationRequestedAsync()
        {
            // given
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask logoutTask = this.careIdentityService.LogoutAsync(cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await logoutTask);

            this.stateBrokerMock.VerifyNoOtherCalls();
            this.tokenBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnCallbackIfCancellationRequestedAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask callbackTask = this.careIdentityService.CallbackAsync(
                randomCode,
                randomState,
                cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await callbackTask);

            this.stateBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnGetAccessTokenIfCancellationRequestedAsync()
        {
            // given
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask<string> getAccessTokenTask =
                this.careIdentityService.GetAccessTokenAsync(cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await getAccessTokenTask);

            this.tokenBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnGetUserInfoIfCancellationRequestedAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityService.GetUserInfoAsync(randomAccessToken, cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await getUserInfoTask);

            this.httpBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionRaisedByABrokerOnGetUserInfoAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityService.GetUserInfoAsync(
                    randomAccessToken,
                    cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await getUserInfoTask);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPropagateCancellationTokenToBrokersOnLogoutAsync()
        {
            // given
            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // when
            await this.careIdentityService.LogoutAsync(cancellationToken);

            // then
            this.stateBrokerMock.Verify(broker =>
                broker.ClearCsrfStateAsync(cancellationToken),
                    Times.Once);

            this.tokenBrokerMock.Verify(broker =>
                broker.ClearAccessTokenAsync(cancellationToken),
                    Times.Once);
        }
    }
}
