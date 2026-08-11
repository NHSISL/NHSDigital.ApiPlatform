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
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnBuildLoginUrlIfDependencyErrorOccursAsync(
            Exception dependencyException)
        {
            // given
            CareIdentityServiceDependencyException expectedCareIdentityServiceDependencyException =
                CreateExpectedDependencyException(dependencyException);

            this.cryptoBrokerMock.Setup(broker =>
                broker.CreateUrlSafeState(It.IsAny<int>()))
                    .Throws(dependencyException);

            // when
            ValueTask<string> buildLoginUrlTask = this.careIdentityService.BuildLoginUrlAsync();

            CareIdentityServiceDependencyException actualCareIdentityServiceDependencyException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await buildLoginUrlTask);

            // then
            actualCareIdentityServiceDependencyException
                .Should().BeEquivalentTo(expectedCareIdentityServiceDependencyException);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowServiceExceptionOnBuildLoginUrlIfServiceErrorOccursAsync(
            Exception serviceException)
        {
            // given
            CareIdentityServiceServiceException expectedCareIdentityServiceServiceException =
                CreateExpectedServiceException(serviceException);

            this.cryptoBrokerMock.Setup(broker =>
                broker.CreateUrlSafeState(It.IsAny<int>()))
                    .Throws(serviceException);

            // when
            ValueTask<string> buildLoginUrlTask = this.careIdentityService.BuildLoginUrlAsync();

            CareIdentityServiceServiceException actualCareIdentityServiceServiceException =
                await Assert.ThrowsAsync<CareIdentityServiceServiceException>(
                    async () => await buildLoginUrlTask);

            // then
            actualCareIdentityServiceServiceException
                .Should().BeEquivalentTo(expectedCareIdentityServiceServiceException);
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnLogoutIfDependencyErrorOccursAsync(
            Exception dependencyException)
        {
            // given
            CareIdentityServiceDependencyException expectedCareIdentityServiceDependencyException =
                CreateExpectedDependencyException(dependencyException);

            this.stateBrokerMock.Setup(broker =>
                broker.ClearCsrfStateAsync(It.IsAny<CancellationToken>()))
                    .Throws(dependencyException);

            // when
            ValueTask logoutTask = this.careIdentityService.LogoutAsync();

            CareIdentityServiceDependencyException actualCareIdentityServiceDependencyException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await logoutTask);

            // then
            actualCareIdentityServiceDependencyException
                .Should().BeEquivalentTo(expectedCareIdentityServiceDependencyException);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowServiceExceptionOnLogoutIfServiceErrorOccursAsync(
            Exception serviceException)
        {
            // given
            CareIdentityServiceServiceException expectedCareIdentityServiceServiceException =
                CreateExpectedServiceException(serviceException);

            this.stateBrokerMock.Setup(broker =>
                broker.ClearCsrfStateAsync(It.IsAny<CancellationToken>()))
                    .Throws(serviceException);

            // when
            ValueTask logoutTask = this.careIdentityService.LogoutAsync();

            CareIdentityServiceServiceException actualCareIdentityServiceServiceException =
                await Assert.ThrowsAsync<CareIdentityServiceServiceException>(
                    async () => await logoutTask);

            // then
            actualCareIdentityServiceServiceException
                .Should().BeEquivalentTo(expectedCareIdentityServiceServiceException);
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnGetUserInfoIfDependencyErrorOccursAsync(
            Exception dependencyException)
        {
            // given
            string randomAccessToken = GetRandomString();

            CareIdentityServiceDependencyException expectedCareIdentityServiceDependencyException =
                CreateExpectedDependencyException(dependencyException);

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(dependencyException);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityService.GetUserInfoAsync(randomAccessToken, default);

            CareIdentityServiceDependencyException actualCareIdentityServiceDependencyException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await getUserInfoTask);

            // then
            actualCareIdentityServiceDependencyException
                .Should().BeEquivalentTo(expectedCareIdentityServiceDependencyException);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowServiceExceptionOnGetUserInfoIfServiceErrorOccursAsync(
            Exception serviceException)
        {
            // given
            string randomAccessToken = GetRandomString();

            CareIdentityServiceServiceException expectedCareIdentityServiceServiceException =
                CreateExpectedServiceException(serviceException);

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(serviceException);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityService.GetUserInfoAsync(randomAccessToken, default);

            CareIdentityServiceServiceException actualCareIdentityServiceServiceException =
                await Assert.ThrowsAsync<CareIdentityServiceServiceException>(
                    async () => await getUserInfoTask);

            // then
            actualCareIdentityServiceServiceException
                .Should().BeEquivalentTo(expectedCareIdentityServiceServiceException);
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnGetAccessTokenIfDependencyErrorOccursAsync(
            Exception dependencyException)
        {
            // given
            CareIdentityServiceDependencyException expectedCareIdentityServiceDependencyException =
                CreateExpectedDependencyException(dependencyException);

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .Throws(dependencyException);

            // when
            ValueTask<string> getAccessTokenTask = this.careIdentityService.GetAccessTokenAsync();

            CareIdentityServiceDependencyException actualCareIdentityServiceDependencyException =
                await Assert.ThrowsAsync<CareIdentityServiceDependencyException>(
                    async () => await getAccessTokenTask);

            // then
            actualCareIdentityServiceDependencyException
                .Should().BeEquivalentTo(expectedCareIdentityServiceDependencyException);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowServiceExceptionOnCallbackIfServiceErrorOccursAsync(
            Exception serviceException)
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();

            CareIdentityServiceServiceException expectedCareIdentityServiceServiceException =
                CreateExpectedServiceException(serviceException);

            this.stateBrokerMock.Setup(broker =>
                broker.GetCsrfStateAsync(It.IsAny<CancellationToken>()))
                    .Throws(serviceException);

            // when
            ValueTask callbackTask = this.careIdentityService.CallbackAsync(randomCode, randomState);

            CareIdentityServiceServiceException actualCareIdentityServiceServiceException =
                await Assert.ThrowsAsync<CareIdentityServiceServiceException>(
                    async () => await callbackTask);

            // then
            actualCareIdentityServiceServiceException
                .Should().BeEquivalentTo(expectedCareIdentityServiceServiceException);
        }

        private static CareIdentityServiceDependencyException CreateExpectedDependencyException(
            Exception dependencyException)
        {
            var failedCareIdentityServiceDependencyException =
                new FailedCareIdentityServiceDependencyException(
                    message: "Failed care identity service dependency error occurred, please contact support.",
                    innerException: dependencyException);

            return new CareIdentityServiceDependencyException(
                message: "Care identity service dependency error occurred, please contact support.",
                innerException: failedCareIdentityServiceDependencyException);
        }

        private static CareIdentityServiceServiceException CreateExpectedServiceException(Exception serviceException)
        {
            var failedCareIdentityServiceException =
                new FailedCareIdentityServiceException(
                    message: "Failed care identity service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            return new CareIdentityServiceServiceException(
                message: "Care identity service error occurred, please contact support.",
                innerException: failedCareIdentityServiceException);
        }
    }
}
