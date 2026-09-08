// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.CareIdentityServices
{
    public partial class CareIdentityServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnBuildLoginUrlIfOperationCanceledExceptionOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            CareIdentityServiceDependencyException expectedCareIdentityServiceDependencyException =
                CreateExpectedTimeoutDependencyException();

            this.cryptoBrokerMock.Setup(broker =>
                broker.CreateUrlSafeState(It.IsAny<int>()))
                    .Throws(operationCanceledException);

            // when
            ValueTask<string> buildLoginUrlTask = this.careIdentityService.BuildLoginUrlAsync();

            CareIdentityServiceDependencyException actualCareIdentityServiceDependencyException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await buildLoginUrlTask);

            // then
            actualCareIdentityServiceDependencyException
                .Should().BeEquivalentTo(expectedCareIdentityServiceDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCareIdentityServiceDependencyException))),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnLogoutIfOperationCanceledExceptionOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            CareIdentityServiceDependencyException expectedCareIdentityServiceDependencyException =
                CreateExpectedTimeoutDependencyException();

            this.stateBrokerMock.Setup(broker =>
                broker.ClearCsrfStateAsync(It.IsAny<CancellationToken>()))
                    .Throws(operationCanceledException);

            // when
            ValueTask logoutTask = this.careIdentityService.LogoutAsync();

            CareIdentityServiceDependencyException actualCareIdentityServiceDependencyException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await logoutTask);

            // then
            actualCareIdentityServiceDependencyException
                .Should().BeEquivalentTo(expectedCareIdentityServiceDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCareIdentityServiceDependencyException))),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnGetUserInfoIfTaskCanceledExceptionOccursAndLogItAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            var taskCanceledException = new TaskCanceledException();

            CareIdentityServiceDependencyException expectedCareIdentityServiceDependencyException =
                CreateExpectedTimeoutDependencyException();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(taskCanceledException);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityService.GetUserInfoAsync(randomAccessToken, default);

            CareIdentityServiceDependencyException actualCareIdentityServiceDependencyException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await getUserInfoTask);

            // then
            actualCareIdentityServiceDependencyException
                .Should().BeEquivalentTo(expectedCareIdentityServiceDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCareIdentityServiceDependencyException))),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnGetAccessTokenIfOperationCanceledOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            CareIdentityServiceDependencyException expectedCareIdentityServiceDependencyException =
                CreateExpectedTimeoutDependencyException();

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .Throws(operationCanceledException);

            // when
            ValueTask<string> getAccessTokenTask = this.careIdentityService.GetAccessTokenAsync();

            CareIdentityServiceDependencyException actualCareIdentityServiceDependencyException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await getAccessTokenTask);

            // then
            actualCareIdentityServiceDependencyException
                .Should().BeEquivalentTo(expectedCareIdentityServiceDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCareIdentityServiceDependencyException))),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnCallbackIfOperationCanceledExceptionOccursAndLogItAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            var operationCanceledException = new OperationCanceledException();

            CareIdentityServiceDependencyException expectedCareIdentityServiceDependencyException =
                CreateExpectedTimeoutDependencyException();

            this.stateBrokerMock.Setup(broker =>
                broker.GetCsrfStateAsync(It.IsAny<CancellationToken>()))
                    .Throws(operationCanceledException);

            // when
            ValueTask callbackTask = this.careIdentityService.CallbackAsync(randomCode, randomState);

            CareIdentityServiceDependencyException actualCareIdentityServiceDependencyException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await callbackTask);

            // then
            actualCareIdentityServiceDependencyException
                .Should().BeEquivalentTo(expectedCareIdentityServiceDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCareIdentityServiceDependencyException))),
                        Times.Once);
        }

        private static CareIdentityServiceDependencyException CreateExpectedTimeoutDependencyException()
        {
            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutCareIdentityServiceException =
                new TimeoutCareIdentityServiceException(
                    message: "Failed care identity service timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            return new CareIdentityServiceDependencyException(
                message: "Care identity service dependency error occurred, please contact support.",
                innerException: timeoutCareIdentityServiceException);
        }
    }
}
