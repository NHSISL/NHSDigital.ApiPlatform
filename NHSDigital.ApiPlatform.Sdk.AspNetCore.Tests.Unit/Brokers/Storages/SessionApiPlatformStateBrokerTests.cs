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
    public class SessionApiPlatformStateBrokerTests
    {
        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
        private readonly FakeSession fakeSession;
        private readonly IApiPlatformStateBroker apiPlatformStateBroker;

        public SessionApiPlatformStateBrokerTests()
        {
            this.httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            this.fakeSession = new FakeSession();

            var httpContext = new DefaultHttpContext
            {
                Session = this.fakeSession
            };

            this.httpContextAccessorMock.Setup(accessor =>
                accessor.HttpContext)
                    .Returns(httpContext);

            this.apiPlatformStateBroker =
                new SessionApiPlatformStateBroker(this.httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task ShouldStoreCsrfStateAsync()
        {
            // given
            string randomState = GetRandomString();

            // when
            await this.apiPlatformStateBroker.StoreCsrfStateAsync(randomState);

            // then
            string actualState = await this.apiPlatformStateBroker.GetCsrfStateAsync();
            actualState.Should().Be(randomState);
        }

        [Fact]
        public async Task ShouldReturnNullOnGetCsrfStateIfStateWasNeverStoredAsync()
        {
            // given
            // when
            string actualState = await this.apiPlatformStateBroker.GetCsrfStateAsync();

            // then
            actualState.Should().BeNull();
        }

        [Fact]
        public async Task ShouldClearCsrfStateAsync()
        {
            // given
            await this.apiPlatformStateBroker.StoreCsrfStateAsync(GetRandomString());

            // when
            await this.apiPlatformStateBroker.ClearCsrfStateAsync();

            // then
            string actualState = await this.apiPlatformStateBroker.GetCsrfStateAsync();
            actualState.Should().BeNull();
        }

        [Fact]
        public async Task ShouldThrowInvalidOperationExceptionOnStoreCsrfStateIfHttpContextIsMissingAsync()
        {
            // given
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            httpContextAccessorMock.Setup(accessor =>
                accessor.HttpContext)
                    .Returns((HttpContext)null);

            IApiPlatformStateBroker stateBroker =
                new SessionApiPlatformStateBroker(httpContextAccessorMock.Object);

            // when
            // then
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await stateBroker.StoreCsrfStateAsync(GetRandomString()));
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 8, wordMaxLength: 12).GetValue();
    }
}
