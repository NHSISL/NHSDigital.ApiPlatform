// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public async Task ShouldStoreAccessTokenOnCallbackAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();

            DateTimeOffset expectedExpiresAtUtc =
                randomDateTimeOffset.AddSeconds(int.Parse(randomTokenResult.ExpiresIn));

            SetupSuccessfulCallback(randomState, randomDateTimeOffset, randomTokenResult, randomUserInfo);

            // when
            await this.careIdentityService.CallbackAsync(randomCode, randomState);

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.StoreAccessTokenAsync(
                    randomTokenResult.AccessToken,
                    expectedExpiresAtUtc,
                    It.IsAny<CancellationToken>()),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldStoreRefreshTokenOnCallbackAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();

            DateTimeOffset expectedExpiresAtUtc =
                randomDateTimeOffset.AddSeconds(int.Parse(randomTokenResult.RefreshTokenExpiresIn));

            SetupSuccessfulCallback(randomState, randomDateTimeOffset, randomTokenResult, randomUserInfo);

            // when
            await this.careIdentityService.CallbackAsync(randomCode, randomState);

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.StoreRefreshTokenAsync(
                    randomTokenResult.RefreshToken,
                    expectedExpiresAtUtc,
                    It.IsAny<CancellationToken>()),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldNotStoreRefreshTokenOnCallbackIfRefreshTokenIsMissingAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            randomTokenResult.RefreshToken = string.Empty;
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();
            SetupSuccessfulCallback(randomState, randomDateTimeOffset, randomTokenResult, randomUserInfo);

            // when
            await this.careIdentityService.CallbackAsync(randomCode, randomState);

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.StoreRefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }

        [Fact]
        public async Task ShouldClearCsrfStateOnCallbackAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();
            SetupSuccessfulCallback(randomState, randomDateTimeOffset, randomTokenResult, randomUserInfo);

            // when
            await this.careIdentityService.CallbackAsync(randomCode, randomState);

            // then
            this.stateBrokerMock.Verify(broker =>
                broker.ClearCsrfStateAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldStoreActiveRoleOnCallbackIfUserHasRolesAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();
            string expectedRoleId = randomUserInfo.NhsIdNrbacRoles[0].PersonRoleId;
            SetupSuccessfulCallback(randomState, randomDateTimeOffset, randomTokenResult, randomUserInfo);

            // when
            await this.careIdentityService.CallbackAsync(randomCode, randomState);

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.StoreActiveRoleAsync(expectedRoleId, It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldNotStoreActiveRoleOnCallbackIfUserHasNoRolesAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();
            randomUserInfo.NhsIdNrbacRoles = new List<NhsNrbacRole>();
            SetupSuccessfulCallback(randomState, randomDateTimeOffset, randomTokenResult, randomUserInfo);

            // when
            await this.careIdentityService.CallbackAsync(randomCode, randomState);

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.StoreActiveRoleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        private void SetupSuccessfulCallback(
            string csrfState,
            DateTimeOffset currentDateTimeOffset,
            TokenResult tokenResult,
            NhsUserInfo userInfo)
        {
            string randomTokenJson = GetRandomString();
            string randomUserInfoJson = GetRandomString();

            this.stateBrokerMock.Setup(broker =>
                broker.GetCsrfStateAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(csrfState);

            this.httpBrokerMock.Setup(broker =>
                broker.PostFormAsync(
                    this.apiPlatformConfigurations.CareIdentity.TokenEndpoint,
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(randomTokenJson));

            this.jsonBrokerMock.Setup(broker =>
                broker.Deserialize<TokenResult>(randomTokenJson))
                    .Returns(tokenResult);

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    this.apiPlatformConfigurations.CareIdentity.UserInfoEndpoint,
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(randomUserInfoJson));

            this.jsonBrokerMock.Setup(broker =>
                broker.Deserialize<NhsUserInfo>(randomUserInfoJson))
                    .Returns(userInfo);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(currentDateTimeOffset);
        }
    }
}
