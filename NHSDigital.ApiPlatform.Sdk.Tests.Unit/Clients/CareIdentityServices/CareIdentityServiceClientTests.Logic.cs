// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        [Fact]
        public async Task ShouldBuildLoginUrlAsync()
        {
            // given
            string randomLoginUrl = GetRandomString();

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomLoginUrl);

            // when
            string actualLoginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();

            // then
            actualLoginUrl.Should().Be(randomLoginUrl);
        }

        [Fact]
        public async Task ShouldLogoutAsync()
        {
            // given
            // when
            await this.careIdentityServiceClient.LogoutAsync();

            // then
            this.careIdentityServiceProcessingServiceMock.Verify(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldGetAccessTokenAsync()
        {
            // given
            string randomAccessToken = GetRandomString();

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomAccessToken);

            // when
            string actualAccessToken = await this.careIdentityServiceClient.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().Be(randomAccessToken);
        }

        [Fact]
        public async Task ShouldGetUserInfoAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();

            this.careIdentityServiceProcessingServiceMock.Setup(service =>
                service.GetUserInfoAsync(randomCode, randomState, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomUserInfo);

            // when
            NhsUserInfo actualUserInfo =
                await this.careIdentityServiceClient.GetUserInfoAsync(randomCode, randomState);

            // then
            actualUserInfo.Should().BeSameAs(randomUserInfo);
        }
    }
}
