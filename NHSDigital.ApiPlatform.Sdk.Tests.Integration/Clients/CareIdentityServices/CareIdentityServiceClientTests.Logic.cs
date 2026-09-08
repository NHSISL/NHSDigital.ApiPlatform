// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration.Clients.CareIdentityServices
{
    public partial class CareIdentityServiceClientTests
    {
        [Fact]
        public async Task ShouldBuildLoginUrlAgainstTheConfiguredAuthEndpointAsync()
        {
            // given
            string expectedAuthEndpoint = this.apiPlatformConfigurations.CareIdentity.AuthEndpoint;

            // when
            string actualLoginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();

            // then
            expectedAuthEndpoint.Should().NotBeNullOrWhiteSpace(
                "appsettings.json must supply the CIS2 authorisation endpoint");

            actualLoginUrl.Should().StartWith(expectedAuthEndpoint);
        }

        [Fact]
        public async Task ShouldIssueAUniqueCsrfStateOnEachBuildLoginUrlAsync()
        {
            // given
            string firstLoginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();

            // when
            string secondLoginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();

            // then
            string firstState = ExtractQueryValue(firstLoginUrl, "state");
            string secondState = ExtractQueryValue(secondLoginUrl, "state");
            firstState.Should().NotBeNullOrWhiteSpace();
            secondState.Should().NotBe(firstState);
        }

        [Fact]
        public async Task ShouldReturnEmptyAccessTokenBeforeAnyLoginAsync()
        {
            // given
            // when
            string actualAccessToken = await this.careIdentityServiceClient.GetAccessTokenAsync();

            // then
            actualAccessToken.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldLogoutWithoutAnEstablishedSessionAsync()
        {
            // given
            // when
            await this.careIdentityServiceClient.LogoutAsync();

            // then
            string actualAccessToken = await this.careIdentityServiceClient.GetAccessTokenAsync();
            actualAccessToken.Should().BeEmpty();
        }

        [Fact(Skip = "Requires NHS CIS2 credentials and an interactive authorisation code.")]
        public async Task ShouldReturnUserInfoOnCompletingTheLoginFlowAsync()
        {
            // given
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractQueryValue(loginUrl, "state");
            string authorisationCode = GetRandomString();

            // when
            NhsUserInfo actualUserInfo =
                await this.careIdentityServiceClient.GetUserInfoAsync(authorisationCode, state);

            // then
            actualUserInfo.Should().NotBeNull();
            actualUserInfo.NhsIdUserUid.Should().NotBeNullOrWhiteSpace();
        }
    }
}
