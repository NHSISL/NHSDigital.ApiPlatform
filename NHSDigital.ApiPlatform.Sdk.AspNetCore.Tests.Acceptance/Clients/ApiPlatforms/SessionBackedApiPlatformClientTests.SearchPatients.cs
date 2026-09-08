// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class SessionBackedApiPlatformClientTests
    {
        [Fact]
        public async Task ShouldSearchPatientsUsingTheSessionStoredCredentialsAsync()
        {
            // given
            string randomNhsNumber = GetRandomNhsNumber();
            string randomAccessToken = GetRandomString();
            string randomRoleId = GetRandomString();
            string randomPatientPayload = $"{{\"resourceType\":\"Patient\",\"id\":\"{randomNhsNumber}\"}}";
            await GivenAnAuthenticatedSessionAsync(randomAccessToken, randomRoleId);
            GivenPatientEndpointReturns(randomNhsNumber, randomPatientPayload);
            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(randomNhsNumber);

            // when
            string actualPayload =
                await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria);

            // then
            actualPayload.Should().Be(randomPatientPayload);

            var patientRequest = this.wireMockServer.LogEntries
                .Last(entry => entry.RequestMessage.Path.EndsWith($"/Patient/{randomNhsNumber}"));

            patientRequest.RequestMessage.Headers["Authorization"]
                .Should().Contain($"Bearer {randomAccessToken}");

            patientRequest.RequestMessage.Headers["NHSD-Session-URID"]
                .Should().Contain(randomRoleId);
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfTheSessionIsNotAuthenticatedAsync()
        {
            // given
            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(GetRandomNhsNumber());

            // when
            PersonalDemographicsServiceClientValidationException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientValidationException>(async () =>
                    await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria));

            // then
            actualException.InnerException.Message
                .Should().Be("Unauthorized - Unable to retrieve access token.");
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnSearchPatientsIfTokenIsAlreadyCancelledAsync()
        {
            // given
            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(GetRandomNhsNumber());
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
                await this.personalDemographicsServiceClient.SearchPatientsAsync(
                    searchCriteria,
                    cancellationTokenSource.Token));
        }

        private async Task GivenAnAuthenticatedSessionAsync(string accessToken, string roleId)
        {
            GivenTokenEndpointReturns(accessToken, GetRandomString());
            GivenUserInfoEndpointReturns(GetRandomString(), roleId);
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);
            await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state);
        }
    }
}
