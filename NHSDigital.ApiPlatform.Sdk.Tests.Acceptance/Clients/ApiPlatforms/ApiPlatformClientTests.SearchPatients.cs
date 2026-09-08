// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class ApiPlatformClientTests
    {
        [Fact]
        public async Task ShouldSearchPatientsByNhsNumberAsync()
        {
            // given
            string randomNhsNumber = GetRandomNhsNumber();
            string randomPatientPayload = $"{{\"resourceType\":\"Patient\",\"id\":\"{randomNhsNumber}\"}}";
            await GivenAnAuthenticatedSessionAsync();
            GivenPatientEndpointReturns(randomNhsNumber, randomPatientPayload);
            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(randomNhsNumber);

            // when
            string actualPayload =
                await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria);

            // then
            actualPayload.Should().Be(randomPatientPayload);
        }

        [Fact]
        public async Task ShouldSendAuthorisationAndSessionHeadersOnSearchPatientsAsync()
        {
            // given
            string randomNhsNumber = GetRandomNhsNumber();
            string randomAccessToken = GetRandomString();
            string randomRoleId = GetRandomString();
            await GivenAnAuthenticatedSessionAsync(randomAccessToken, randomRoleId);
            GivenPatientEndpointReturns(randomNhsNumber, "{}");
            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(randomNhsNumber);

            // when
            await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria);

            // then
            var patientRequest = this.wireMockServer.LogEntries
                .Last(entry => entry.RequestMessage.Path.EndsWith($"/Patient/{randomNhsNumber}"));

            patientRequest.RequestMessage.Headers["Authorization"]
                .Should().Contain($"Bearer {randomAccessToken}");

            patientRequest.RequestMessage.Headers["NHSD-Session-URID"]
                .Should().Contain(randomRoleId);

            patientRequest.RequestMessage.Headers.Should().ContainKey("X-Request-ID");
        }

        private async Task GivenAnAuthenticatedSessionAsync(
            string accessToken = null,
            string roleId = null)
        {
            GivenTokenEndpointReturns(
                accessToken: accessToken ?? GetRandomString(),
                refreshToken: GetRandomString());

            GivenUserInfoEndpointReturns(GetRandomString(), roleId ?? GetRandomString());
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);
            await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state);
        }
    }
}
