// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
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
        [MemberData(nameof(InvalidTexts))]
        public async Task ShouldThrowValidationExceptionOnCallbackIfCodeIsInvalidAsync(string invalidCode)
        {
            // given
            string randomState = GetRandomString();

            var invalidArgumentCareIdentityServiceException =
                new InvalidArgumentCareIdentityServiceException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentCareIdentityServiceException.UpsertDataList(
                key: "code",
                value: "Text is required");

            var expectedCareIdentityServiceValidationException =
                new CareIdentityServiceValidationException(
                    message: "Care identity service validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: invalidArgumentCareIdentityServiceException);

            // when
            ValueTask callbackTask =
                this.careIdentityService.CallbackAsync(invalidCode, randomState);

            CareIdentityServiceValidationException actualCareIdentityServiceValidationException =
                await Assert.ThrowsAsync<CareIdentityServiceValidationException>(
                    async () => await callbackTask);

            // then
            actualCareIdentityServiceValidationException
                .Should().BeEquivalentTo(expectedCareIdentityServiceValidationException);
        }

        [Theory]
        [MemberData(nameof(InvalidTexts))]
        public async Task ShouldThrowValidationExceptionOnCallbackIfStateIsInvalidAsync(string invalidState)
        {
            // given
            string randomCode = GetRandomString();

            var invalidArgumentCareIdentityServiceException =
                new InvalidArgumentCareIdentityServiceException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentCareIdentityServiceException.UpsertDataList(
                key: "state",
                value: "Text is required");

            var expectedCareIdentityServiceValidationException =
                new CareIdentityServiceValidationException(
                    message: "Care identity service validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: invalidArgumentCareIdentityServiceException);

            // when
            ValueTask callbackTask =
                this.careIdentityService.CallbackAsync(randomCode, invalidState);

            CareIdentityServiceValidationException actualCareIdentityServiceValidationException =
                await Assert.ThrowsAsync<CareIdentityServiceValidationException>(
                    async () => await callbackTask);

            // then
            actualCareIdentityServiceValidationException
                .Should().BeEquivalentTo(expectedCareIdentityServiceValidationException);
        }

        [Theory]
        [MemberData(nameof(InvalidTexts))]
        public async Task ShouldThrowValidationExceptionOnGetUserInfoIfAccessTokenIsInvalidAsync(
            string invalidAccessToken)
        {
            // given
            var invalidArgumentCareIdentityServiceException =
                new InvalidArgumentCareIdentityServiceException(
                    message: "Invalid argument(s), please correct the errors and try again.");

            invalidArgumentCareIdentityServiceException.UpsertDataList(
                key: "accessToken",
                value: "Text is required");

            var expectedCareIdentityServiceValidationException =
                new CareIdentityServiceValidationException(
                    message: "Care identity service validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: invalidArgumentCareIdentityServiceException);

            // when
            ValueTask<NhsUserInfo> getUserInfoTask =
                this.careIdentityService.GetUserInfoAsync(invalidAccessToken, default);

            CareIdentityServiceValidationException actualCareIdentityServiceValidationException =
                await Assert.ThrowsAsync<CareIdentityServiceValidationException>(
                    async () => await getUserInfoTask);

            // then
            actualCareIdentityServiceValidationException
                .Should().BeEquivalentTo(expectedCareIdentityServiceValidationException);

            this.httpBrokerMock.Verify(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<System.Net.Http.HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnCallbackIfStateDoesNotMatchStoredStateAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            string differentState = GetRandomString();

            var invalidStateCareIdentityServiceException =
                new InvalidStateCareIdentityServiceException(
                    message: "Invalid state parameter.");

            var expectedCareIdentityServiceValidationException =
                new CareIdentityServiceValidationException(
                    message: "Care identity service validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: invalidStateCareIdentityServiceException);

            this.stateBrokerMock.Setup(broker =>
                broker.GetCsrfStateAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(differentState);

            // when
            ValueTask callbackTask = this.careIdentityService.CallbackAsync(randomCode, randomState);

            CareIdentityServiceValidationException actualCareIdentityServiceValidationException =
                await Assert.ThrowsAsync<CareIdentityServiceValidationException>(
                    async () => await callbackTask);

            // then
            actualCareIdentityServiceValidationException
                .Should().BeEquivalentTo(expectedCareIdentityServiceValidationException);

            this.stateBrokerMock.Verify(broker =>
                broker.ClearCsrfStateAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnCallbackIfNoStateWasStoredAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();

            var invalidStateCareIdentityServiceException =
                new InvalidStateCareIdentityServiceException(
                    message: "Invalid state parameter.");

            var expectedCareIdentityServiceValidationException =
                new CareIdentityServiceValidationException(
                    message: "Care identity service validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: invalidStateCareIdentityServiceException);

            this.stateBrokerMock.Setup(broker =>
                broker.GetCsrfStateAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((string)null);

            // when
            ValueTask callbackTask = this.careIdentityService.CallbackAsync(randomCode, randomState);

            CareIdentityServiceValidationException actualCareIdentityServiceValidationException =
                await Assert.ThrowsAsync<CareIdentityServiceValidationException>(
                    async () => await callbackTask);

            // then
            actualCareIdentityServiceValidationException
                .Should().BeEquivalentTo(expectedCareIdentityServiceValidationException);
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnGetAccessTokenIfRefreshedTokenIsEmptyAsync()
        {
            // given
            string randomRefreshToken = GetRandomString();
            string randomTokenJson = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult emptyTokenResult = CreateRandomTokenResult();
            emptyTokenResult.AccessToken = string.Empty;

            var unauthorisedCareIdentityServiceException =
                new UnauthorisedCareIdentityServiceException(
                    message: "Authentication failed (no access token).");

            var expectedCareIdentityServiceValidationException =
                new CareIdentityServiceValidationException(
                    message: "Care identity service validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: unauthorisedCareIdentityServiceException);

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((null, null));

            this.tokenBrokerMock.Setup(broker =>
                broker.GetRefreshTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((randomRefreshToken, randomDateTimeOffset.AddMinutes(30)));

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.httpBrokerMock.Setup(broker =>
                broker.PostFormAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(randomTokenJson));

            this.jsonBrokerMock.Setup(broker =>
                broker.Deserialize<TokenResult>(randomTokenJson))
                    .Returns(emptyTokenResult);

            // when
            ValueTask<string> getAccessTokenTask = this.careIdentityService.GetAccessTokenAsync();

            CareIdentityServiceValidationException actualCareIdentityServiceValidationException =
                await Assert.ThrowsAsync<CareIdentityServiceValidationException>(
                    async () => await getAccessTokenTask);

            // then
            actualCareIdentityServiceValidationException
                .Should().BeEquivalentTo(expectedCareIdentityServiceValidationException);
        }
    }
}
