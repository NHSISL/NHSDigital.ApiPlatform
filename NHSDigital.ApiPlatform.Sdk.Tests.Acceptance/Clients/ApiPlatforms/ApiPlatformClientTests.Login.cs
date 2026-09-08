// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class ApiPlatformClientTests
    {
        [Fact]
        public async Task ShouldBuildLoginUrlAsync()
        {
            // given
            // when
            string actualLoginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();

            // then
            actualLoginUrl.Should().StartWith(this.apiPlatformConfigurations.CareIdentity.AuthEndpoint);
            actualLoginUrl.Should().Contain($"client_id={this.apiPlatformConfigurations.CareIdentity.ClientId}");
            actualLoginUrl.Should().Contain("response_type=code");
            ExtractStateFromLoginUrl(actualLoginUrl).Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ShouldReturnUserInfoOnCompletingTheLoginFlowAsync()
        {
            // given
            string randomUserUid = GetRandomString();
            string randomRoleId = GetRandomString();
            GivenTokenEndpointReturns(accessToken: GetRandomString(), refreshToken: GetRandomString());
            GivenUserInfoEndpointReturns(randomUserUid, randomRoleId);
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);

            // when
            NhsUserInfo actualUserInfo =
                await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state);

            // then
            actualUserInfo.NhsIdUserUid.Should().Be(randomUserUid);
            actualUserInfo.NhsIdNrbacRoles.Should().ContainSingle();
            actualUserInfo.NhsIdNrbacRoles[0].PersonRoleId.Should().Be(randomRoleId);
        }

        [Fact]
        public async Task ShouldReturnAccessTokenAfterCompletingTheLoginFlowAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            GivenTokenEndpointReturns(randomAccessToken, refreshToken: GetRandomString());
            GivenUserInfoEndpointReturns(GetRandomString(), GetRandomString());
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);
            await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state);

            // when
            string actualAccessToken = await this.careIdentityServiceClient.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().Be(randomAccessToken);
        }

        [Fact]
        public async Task ShouldReturnEmptyAccessTokenBeforeLoggingInAsync()
        {
            // given
            // when
            string actualAccessToken = await this.careIdentityServiceClient.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldDiscardAccessTokenOnLogoutAsync()
        {
            // given
            GivenTokenEndpointReturns(GetRandomString(), refreshToken: GetRandomString());
            GivenUserInfoEndpointReturns(GetRandomString(), GetRandomString());
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);
            await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state);

            // when
            await this.careIdentityServiceClient.LogoutAsync();

            // then
            string actualAccessToken = await this.careIdentityServiceClient.GetAccessTokenAsync();
            actualAccessToken.Should().BeEmpty();
        }
    }
}
