// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSDigital.ApiPlatform.Sdk.AspNetCore.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using Tynamix.ObjectFiller;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Tests.Unit.Brokers.Storages
{
    public class SessionApiPlatformTokenBrokerTests
    {
        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
        private readonly IApiPlatformTokenBroker apiPlatformTokenBroker;

        public SessionApiPlatformTokenBrokerTests()
        {
            this.httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var httpContext = new DefaultHttpContext
            {
                Session = new FakeSession()
            };

            this.httpContextAccessorMock.Setup(accessor =>
                accessor.HttpContext)
                    .Returns(httpContext);

            this.apiPlatformTokenBroker =
                new SessionApiPlatformTokenBroker(this.httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task ShouldStoreAccessTokenAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            DateTimeOffset randomExpiresAtUtc = GetRandomDateTimeOffset();

            // when
            await this.apiPlatformTokenBroker.StoreAccessTokenAsync(randomAccessToken, randomExpiresAtUtc);

            // then
            var (actualToken, _) = await this.apiPlatformTokenBroker.GetAccessTokenAsync();
            actualToken.Should().Be(randomAccessToken);
        }

        [Fact]
        public async Task ShouldStoreAccessTokenExpiryToTheSecondAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            DateTimeOffset randomExpiresAtUtc = GetRandomDateTimeOffset();

            DateTimeOffset expectedExpiresAtUtc =
                DateTimeOffset.FromUnixTimeSeconds(randomExpiresAtUtc.ToUnixTimeSeconds());

            // when
            await this.apiPlatformTokenBroker.StoreAccessTokenAsync(randomAccessToken, randomExpiresAtUtc);

            // then
            var (_, actualExpiresAtUtc) = await this.apiPlatformTokenBroker.GetAccessTokenAsync();
            actualExpiresAtUtc.Should().Be(expectedExpiresAtUtc);
        }

        [Fact]
        public async Task ShouldReturnNullsOnGetAccessTokenIfTokenWasNeverStoredAsync()
        {
            // given
            // when
            var (actualToken, actualExpiresAtUtc) = await this.apiPlatformTokenBroker.GetAccessTokenAsync();

            // then
            actualToken.Should().BeNull();
            actualExpiresAtUtc.Should().BeNull();
        }

        [Fact]
        public async Task ShouldClearAccessTokenAsync()
        {
            // given
            await this.apiPlatformTokenBroker.StoreAccessTokenAsync(
                GetRandomString(),
                GetRandomDateTimeOffset());

            // when
            await this.apiPlatformTokenBroker.ClearAccessTokenAsync();

            // then
            var (actualToken, actualExpiresAtUtc) = await this.apiPlatformTokenBroker.GetAccessTokenAsync();
            actualToken.Should().BeNull();
            actualExpiresAtUtc.Should().BeNull();
        }

        [Fact]
        public async Task ShouldStoreRefreshTokenAsync()
        {
            // given
            string randomRefreshToken = GetRandomString();
            DateTimeOffset randomExpiresAtUtc = GetRandomDateTimeOffset();

            // when
            await this.apiPlatformTokenBroker.StoreRefreshTokenAsync(randomRefreshToken, randomExpiresAtUtc);

            // then
            var (actualToken, _) = await this.apiPlatformTokenBroker.GetRefreshTokenAsync();
            actualToken.Should().Be(randomRefreshToken);
        }

        [Fact]
        public async Task ShouldStoreRefreshTokenExpiryToTheSecondAsync()
        {
            // given
            // This expiry is the sole input to the decision to silently refresh or sign the user out,
            // so the round trip through the session has to preserve it.
            string randomRefreshToken = GetRandomString();
            DateTimeOffset randomExpiresAtUtc = GetRandomDateTimeOffset();

            DateTimeOffset expectedExpiresAtUtc =
                DateTimeOffset.FromUnixTimeSeconds(randomExpiresAtUtc.ToUnixTimeSeconds());

            // when
            await this.apiPlatformTokenBroker.StoreRefreshTokenAsync(randomRefreshToken, randomExpiresAtUtc);

            // then
            var (_, actualExpiresAtUtc) = await this.apiPlatformTokenBroker.GetRefreshTokenAsync();
            actualExpiresAtUtc.Should().Be(expectedExpiresAtUtc);
        }

        [Fact]
        public async Task ShouldReturnNullsOnGetRefreshTokenIfTokenWasNeverStoredAsync()
        {
            // given
            // when
            var (actualToken, actualExpiresAtUtc) = await this.apiPlatformTokenBroker.GetRefreshTokenAsync();

            // then
            actualToken.Should().BeNull();
            actualExpiresAtUtc.Should().BeNull();
        }

        [Fact]
        public async Task ShouldClearRefreshTokenAsync()
        {
            // given
            await this.apiPlatformTokenBroker.StoreRefreshTokenAsync(
                GetRandomString(),
                GetRandomDateTimeOffset());

            // when
            await this.apiPlatformTokenBroker.ClearRefreshTokenAsync();

            // then
            var (actualToken, actualExpiresAtUtc) = await this.apiPlatformTokenBroker.GetRefreshTokenAsync();
            actualToken.Should().BeNull();
            actualExpiresAtUtc.Should().BeNull();
        }

        [Fact]
        public async Task ShouldStoreActiveRoleAsync()
        {
            // given
            string randomRoleId = GetRandomString();

            // when
            await this.apiPlatformTokenBroker.StoreActiveRoleAsync(randomRoleId);

            // then
            string actualRoleId = await this.apiPlatformTokenBroker.GetActiveRoleAsync();
            actualRoleId.Should().Be(randomRoleId);
        }

        [Fact]
        public async Task ShouldClearActiveRoleAsync()
        {
            // given
            await this.apiPlatformTokenBroker.StoreActiveRoleAsync(GetRandomString());

            // when
            await this.apiPlatformTokenBroker.ClearActiveRoleAsync();

            // then
            string actualRoleId = await this.apiPlatformTokenBroker.GetActiveRoleAsync();
            actualRoleId.Should().BeNull();
        }

        [Fact]
        public async Task ShouldThrowInvalidOperationExceptionOnGetAccessTokenIfHttpContextIsMissingAsync()
        {
            // given
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            httpContextAccessorMock.Setup(accessor =>
                accessor.HttpContext)
                    .Returns((HttpContext)null);

            IApiPlatformTokenBroker tokenBroker =
                new SessionApiPlatformTokenBroker(httpContextAccessorMock.Object);

            // when
            // then
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await tokenBroker.GetAccessTokenAsync());
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 8, wordMaxLength: 12).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime(2020, 1, 1)).GetValue();
    }
}
