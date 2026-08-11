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
        public async Task ShouldThrowOperationCanceledExceptionOnBuildLoginUrlIfTokenIsAlreadyCancelledAsync()
        {
            // given
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            ValueTask<string> buildLoginUrlTask =
                this.careIdentityService.BuildLoginUrlAsync(cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await buildLoginUrlTask);

            this.cryptoBrokerMock.Verify(broker =>
                broker.CreateUrlSafeState(It.IsAny<int>()),
                    Times.Never);
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnBuildLoginUrlAsync()
        {
            // given
            this.cryptoBrokerMock.Setup(broker =>
                broker.CreateUrlSafeState(It.IsAny<int>()))
                    .Throws(new OperationCanceledException());

            // when
            ValueTask<string> buildLoginUrlTask = this.careIdentityService.BuildLoginUrlAsync();

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await buildLoginUrlTask);
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnLogoutAsync()
        {
            // given
            this.stateBrokerMock.Setup(broker =>
                broker.ClearCsrfStateAsync(It.IsAny<CancellationToken>()))
                    .Throws(new OperationCanceledException());

            // when
            ValueTask logoutTask = this.careIdentityService.LogoutAsync();

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await logoutTask);
        }

        [Fact]
        public async Task ShouldNotWrapTaskCanceledExceptionOnGetUserInfoAsync()
        {
            // given
            string randomAccessToken = GetRandomString();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(new TaskCanceledException());

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityService.GetUserInfoAsync(randomAccessToken, default);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await getUserInfoTask);
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnGetAccessTokenIfTokenIsAlreadyCancelledAsync()
        {
            // given
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            ValueTask<string> getAccessTokenTask =
                this.careIdentityService.GetAccessTokenAsync(cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await getAccessTokenTask);

            this.tokenBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnCallbackIfTokenIsAlreadyCancelledAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            ValueTask callbackTask = this.careIdentityService.CallbackAsync(
                randomCode,
                randomState,
                cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await callbackTask);

            this.stateBrokerMock.Verify(broker =>
                broker.GetCsrfStateAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
        }
    }
}
