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
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.CareIdentityServices
{
    public partial class CareIdentityServiceTests
    {
        [Fact]
        public async Task ShouldReturnStoredAccessTokenOnGetAccessTokenIfTokenIsStillValidAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset expiresAtUtc = randomDateTimeOffset.AddMinutes(30);

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((randomAccessToken, expiresAtUtc));

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            string actualAccessToken = await this.careIdentityService.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().Be(randomAccessToken);
        }

        [Fact]
        public async Task ShouldReturnEmptyStringOnGetAccessTokenIfRefreshTokenIsMissingAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((null, null));

            this.tokenBrokerMock.Setup(broker =>
                broker.GetRefreshTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((null, null));

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            string actualAccessToken = await this.careIdentityService.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldReturnEmptyStringOnGetAccessTokenIfRefreshTokenHasExpiredAsync()
        {
            // given
            string randomRefreshToken = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset expiredAtUtc = randomDateTimeOffset.AddMinutes(-1);

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((null, null));

            this.tokenBrokerMock.Setup(broker =>
                broker.GetRefreshTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((randomRefreshToken, expiredAtUtc));

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            string actualAccessToken = await this.careIdentityService.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldRefreshAccessTokenOnGetAccessTokenIfStoredTokenHasExpiredAsync()
        {
            // given
            string randomRefreshToken = GetRandomString();
            string expiredAccessToken = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();

            SetupExpiredAccessTokenWithValidRefreshToken(
                randomRefreshToken,
                randomDateTimeOffset,
                randomTokenResult,
                storedAccessToken: expiredAccessToken,
                storedAccessExpiresAtUtc: randomDateTimeOffset.AddSeconds(-1));

            // when
            string actualAccessToken = await this.careIdentityService.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().Be(randomTokenResult.AccessToken);
            actualAccessToken.Should().NotBe(expiredAccessToken);
        }

        [Fact]
        public async Task ShouldRefreshAccessTokenOnGetAccessTokenIfStoredTokenExpiresInsideTheRefreshSkewAsync()
        {
            // given
            string randomRefreshToken = GetRandomString();
            string nearlyExpiredAccessToken = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();

            // The service refreshes anything expiring within 60 seconds rather than handing back a token
            // that will die mid-request.
            SetupExpiredAccessTokenWithValidRefreshToken(
                randomRefreshToken,
                randomDateTimeOffset,
                randomTokenResult,
                storedAccessToken: nearlyExpiredAccessToken,
                storedAccessExpiresAtUtc: randomDateTimeOffset.AddSeconds(59));

            // when
            string actualAccessToken = await this.careIdentityService.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().Be(randomTokenResult.AccessToken);
            actualAccessToken.Should().NotBe(nearlyExpiredAccessToken);
        }

        [Fact]
        public async Task ShouldReturnStoredAccessTokenOnGetAccessTokenIfItOutlivesTheRefreshSkewAsync()
        {
            // given
            string storedAccessToken = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((storedAccessToken, randomDateTimeOffset.AddSeconds(61)));

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            string actualAccessToken = await this.careIdentityService.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().Be(storedAccessToken);

            this.tokenBrokerMock.Verify(broker =>
                broker.GetRefreshTokenAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ShouldStoreRefreshedAccessTokenOnGetAccessTokenAsync()
        {
            // given
            string randomRefreshToken = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();

            DateTimeOffset expectedExpiresAtUtc =
                randomDateTimeOffset.AddSeconds(int.Parse(randomTokenResult.ExpiresIn));

            SetupExpiredAccessTokenWithValidRefreshToken(randomRefreshToken, randomDateTimeOffset, randomTokenResult);

            // when
            await this.careIdentityService.GetAccessTokenAsync();

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.StoreAccessTokenAsync(
                    randomTokenResult.AccessToken,
                    expectedExpiresAtUtc,
                    It.IsAny<CancellationToken>()),
                        Times.Once);
        }

        private void SetupExpiredAccessTokenWithValidRefreshToken(
            string refreshToken,
            DateTimeOffset currentDateTimeOffset,
            TokenResult refreshedTokenResult,
            string storedAccessToken = null,
            DateTimeOffset? storedAccessExpiresAtUtc = null)
        {
            string randomTokenJson = GetRandomString();

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((storedAccessToken, storedAccessExpiresAtUtc));

            this.tokenBrokerMock.Setup(broker =>
                broker.GetRefreshTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((refreshToken, currentDateTimeOffset.AddMinutes(30)));

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(currentDateTimeOffset);

            this.httpBrokerMock.Setup(broker =>
                broker.PostFormAsync(
                    this.apiPlatformConfigurations.CareIdentity.TokenEndpoint,
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(randomTokenJson));

            this.jsonBrokerMock.Setup(broker =>
                broker.Deserialize<TokenResult>(randomTokenJson))
                    .Returns(refreshedTokenResult);
        }

        [Fact]
        public async Task ShouldStoreTheRotatedRefreshTokenOnGetAccessTokenAsync()
        {
            // given
            // CIS2 rotates the refresh token on every refresh. If the SDK fails to store the new one,
            // the next refresh presents a spent token and the user is silently signed out.
            string randomRefreshToken = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();

            DateTimeOffset expectedRefreshExpiresAtUtc =
                randomDateTimeOffset.AddSeconds(int.Parse(randomTokenResult.RefreshTokenExpiresIn));

            SetupExpiredAccessTokenWithValidRefreshToken(
                randomRefreshToken,
                randomDateTimeOffset,
                randomTokenResult);

            // when
            await this.careIdentityService.GetAccessTokenAsync();

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.StoreRefreshTokenAsync(
                    randomTokenResult.RefreshToken,
                    expectedRefreshExpiresAtUtc,
                    It.IsAny<CancellationToken>()),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldNotStoreARefreshTokenOnGetAccessTokenIfTheRefreshDidNotReturnOneAsync()
        {
            // given
            string randomRefreshToken = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            randomTokenResult.RefreshToken = string.Empty;

            SetupExpiredAccessTokenWithValidRefreshToken(
                randomRefreshToken,
                randomDateTimeOffset,
                randomTokenResult);

            // when
            await this.careIdentityService.GetAccessTokenAsync();

            // then
            this.tokenBrokerMock.Verify(broker =>
                broker.StoreRefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }
    }
}
