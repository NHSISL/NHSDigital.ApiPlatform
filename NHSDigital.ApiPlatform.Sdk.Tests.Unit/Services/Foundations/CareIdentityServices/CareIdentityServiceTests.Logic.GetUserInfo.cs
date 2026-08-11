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
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.CareIdentityServices
{
    public partial class CareIdentityServiceTests
    {
        [Fact]
        public async Task ShouldReturnUserInfoOnGetUserInfoAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string randomUserInfoJson = GetRandomString();
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    this.apiPlatformConfigurations.CareIdentity.UserInfoEndpoint,
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(randomUserInfoJson));

            this.jsonBrokerMock.Setup(broker =>
                broker.Deserialize<NhsUserInfo>(randomUserInfoJson))
                    .Returns(randomUserInfo);

            // when
            NhsUserInfo actualUserInfo =
                await this.careIdentityService.GetUserInfoAsync(randomAccessToken, default);

            // then
            actualUserInfo.Should().BeSameAs(randomUserInfo);
        }

        [Fact]
        public async Task ShouldCallUserInfoEndpointOnGetUserInfoAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string randomUserInfoJson = GetRandomString();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(randomUserInfoJson));

            this.jsonBrokerMock.Setup(broker =>
                broker.Deserialize<NhsUserInfo>(randomUserInfoJson))
                    .Returns(CreateRandomNhsUserInfo());

            // when
            await this.careIdentityService.GetUserInfoAsync(randomAccessToken, default);

            // then
            this.httpBrokerMock.Verify(broker =>
                broker.GetAsync(
                    this.apiPlatformConfigurations.CareIdentity.UserInfoEndpoint,
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);
        }
    }
}
