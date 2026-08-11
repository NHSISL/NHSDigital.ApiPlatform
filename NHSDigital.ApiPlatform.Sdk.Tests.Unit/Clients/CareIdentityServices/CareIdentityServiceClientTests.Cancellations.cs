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
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        [Fact]
        public async Task ShouldThrowClientDependencyExceptionOnBuildLoginUrlIfOperationCanceledExceptionOccursAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            CareIdentityServiceClientDependencyException expectedException =
                CreateExpectedTimeoutDependencyException();

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<string> buildLoginUrlTask = this.careIdentityServiceClient.BuildLoginUrlAsync();

            CareIdentityServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyException>(
                    async () => await buildLoginUrlTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Fact]
        public async Task ShouldThrowClientDependencyExceptionOnGetAccessTokenIfTaskCanceledExceptionOccursAsync()
        {
            // given
            var taskCanceledException = new TaskCanceledException();

            CareIdentityServiceClientDependencyException expectedException =
                CreateExpectedTimeoutDependencyException();

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(taskCanceledException);

            // when
            ValueTask<string> getAccessTokenTask = this.careIdentityServiceClient.GetAccessTokenAsync();

            CareIdentityServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyException>(
                    async () => await getAccessTokenTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnBuildLoginUrlIfCancellationRequestedAsync()
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
        public async Task ShouldNotWrapOperationCanceledExceptionOnLogoutIfCancellationRequestedAsync()
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
        public async Task ShouldNotWrapOperationCanceledExceptionOnGetUserInfoIfCancellationRequestedAsync()
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

        private static CareIdentityServiceClientDependencyException CreateExpectedTimeoutDependencyException()
        {
            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutCareIdentityServiceClientException =
                new TimeoutCareIdentityServiceClientException(
                    message: "Failed care identity service client timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            return new CareIdentityServiceClientDependencyException(
                message: "Care identity service client dependency error occurred, contact support.",
                innerException: timeoutCareIdentityServiceClientException);
        }
    }
}
