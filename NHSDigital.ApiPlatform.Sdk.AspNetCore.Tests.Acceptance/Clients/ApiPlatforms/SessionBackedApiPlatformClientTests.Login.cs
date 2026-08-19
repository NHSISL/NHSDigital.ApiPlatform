// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class SessionBackedApiPlatformClientTests
    {
        [Fact]
        public async Task ShouldPersistCsrfStateInTheSessionOnBuildLoginUrlAsync()
        {
            // given
            // when
            string actualLoginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();

            // then
            string state = ExtractStateFromLoginUrl(actualLoginUrl);
            state.Should().NotBeNullOrWhiteSpace();
            this.fakeSession.Keys.Should().Contain("Nhs.ApiPlatform.CsrfState");
        }

        [Fact]
        public async Task ShouldPersistTokensInTheSessionOnCompletingTheLoginFlowAsync()
        {
            // given
            GivenTokenEndpointReturns(GetRandomString(), GetRandomString());
            GivenUserInfoEndpointReturns(GetRandomString(), GetRandomString());
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);

            // when
            await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state);

            // then
            this.fakeSession.Keys.Should().Contain("Nhs.ApiPlatform.AccessToken");
            this.fakeSession.Keys.Should().Contain("Nhs.ApiPlatform.RefreshToken");
            this.fakeSession.Keys.Should().Contain("Nhs.ApiPlatform.ActiveRoleId");
        }

        [Fact]
        public async Task ShouldReturnUserInfoOnCompletingTheLoginFlowAsync()
        {
            // given
            string randomUserUid = GetRandomString();
            string randomRoleId = GetRandomString();
            GivenTokenEndpointReturns(GetRandomString(), GetRandomString());
            GivenUserInfoEndpointReturns(randomUserUid, randomRoleId);
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);

            // when
            NhsUserInfo actualUserInfo =
                await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state);

            // then
            actualUserInfo.NhsIdUserUid.Should().Be(randomUserUid);
            actualUserInfo.NhsIdNrbacRoles.Single().PersonRoleId.Should().Be(randomRoleId);
        }

        [Fact]
        public async Task ShouldReturnSessionStoredAccessTokenOnGetAccessTokenAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            GivenTokenEndpointReturns(randomAccessToken, GetRandomString());
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
        public async Task ShouldRemoveTokensFromTheSessionOnLogoutAsync()
        {
            // given
            GivenTokenEndpointReturns(GetRandomString(), GetRandomString());
            GivenUserInfoEndpointReturns(GetRandomString(), GetRandomString());
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);
            await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state);

            // when
            await this.careIdentityServiceClient.LogoutAsync();

            // then
            this.fakeSession.Keys.Should().NotContain("Nhs.ApiPlatform.AccessToken");
            this.fakeSession.Keys.Should().NotContain("Nhs.ApiPlatform.RefreshToken");
            this.fakeSession.Keys.Should().NotContain("Nhs.ApiPlatform.ActiveRoleId");
            this.fakeSession.Keys.Should().NotContain("Nhs.ApiPlatform.CsrfState");
        }
    }
}
