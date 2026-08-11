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
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TokenResult randomTokenResult = CreateRandomTokenResult();
            SetupExpiredAccessTokenWithValidRefreshToken(randomRefreshToken, randomDateTimeOffset, randomTokenResult);

            // when
            string actualAccessToken = await this.careIdentityService.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().Be(randomTokenResult.AccessToken);
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
            TokenResult refreshedTokenResult)
        {
            string randomTokenJson = GetRandomString();

            this.tokenBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync((null, null));

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
    }
}
