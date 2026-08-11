// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnBuildLoginUrlAsync()
        {
            // given
            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException());

            // when
            ValueTask<string> buildLoginUrlTask = this.careIdentityServiceClient.BuildLoginUrlAsync();

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await buildLoginUrlTask);
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnLogoutAsync()
        {
            // given
            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException());

            // when
            ValueTask logoutTask = this.careIdentityServiceClient.LogoutAsync();

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await logoutTask);
        }

        [Fact]
        public async Task ShouldNotWrapTaskCanceledExceptionOnGetAccessTokenAsync()
        {
            // given
            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new TaskCanceledException());

            // when
            ValueTask<string> getAccessTokenTask = this.careIdentityServiceClient.GetAccessTokenAsync();

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await getAccessTokenTask);
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
