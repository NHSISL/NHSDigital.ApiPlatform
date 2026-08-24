// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Processings.CareIdentityServices
{
    public partial class CareIdentityServiceProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBuildLoginUrlAsync()
        {
            // given
            string randomLoginUrl = GetRandomString();

            this.careIdentityServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomLoginUrl);

            // when
            string actualLoginUrl = await this.careIdentityServiceProcessingService.BuildLoginUrlAsync();

            // then
            actualLoginUrl.Should().Be(randomLoginUrl);

            this.careIdentityServiceMock.Verify(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldLogoutAsync()
        {
            // given
            // when
            await this.careIdentityServiceProcessingService.LogoutAsync();

            // then
            this.careIdentityServiceMock.Verify(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldGetAccessTokenAsync()
        {
            // given
            string randomAccessToken = GetRandomString();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomAccessToken);

            // when
            string actualAccessToken = await this.careIdentityServiceProcessingService.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().Be(randomAccessToken);
        }

        [Fact]
        public async Task ShouldReturnUserInfoOnGetUserInfoAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            string randomAccessToken = GetRandomString();
            NhsUserInfo randomUserInfo = CreateRandomNhsUserInfo();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomAccessToken);

            this.careIdentityServiceMock.Setup(service =>
                service.GetUserInfoAsync(randomAccessToken, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomUserInfo);

            // when
            NhsUserInfo actualUserInfo =
                await this.careIdentityServiceProcessingService.GetUserInfoAsync(randomCode, randomState);

            // then
            actualUserInfo.Should().BeSameAs(randomUserInfo);
        }

        [Fact]
        public async Task ShouldCompleteCallbackBeforeRetrievingUserInfoOnGetUserInfoAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            string randomAccessToken = GetRandomString();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomAccessToken);

            this.careIdentityServiceMock.Setup(service =>
                service.GetUserInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(CreateRandomNhsUserInfo());

            // when
            await this.careIdentityServiceProcessingService.GetUserInfoAsync(randomCode, randomState);

            // then
            this.careIdentityServiceMock.Verify(service =>
                service.CallbackAsync(randomCode, randomState, It.IsAny<CancellationToken>()),
                    Times.Once);
        }
    }
}
